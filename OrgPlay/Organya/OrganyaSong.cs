using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

// TODO: Clean this up and stuff.
// I hate MonoDevelop.

namespace OrgPlay.Organya
{
    public class OrganyaSong
    {
        public readonly ImmutableList<OrganyaInstrument> Tracks;
        public readonly int StepLengthMsec;
        public readonly int StepsPerBeat;
        public readonly int BeatsPerBar;
        public readonly int LoopStartPositionSteps;
        public readonly int LoopEndPositionSteps;

        public OrganyaSong(int clickLengthMsec, int clicksPerBeat, int beatsPerBar, int loopStart, int loopEnd, IEnumerable<OrganyaInstrument> tracks)
        {
            Tracks = tracks.ToImmutableList();

            if (Tracks.Count != 16)
            {
                throw new ArgumentException("Received an unexpected track count.");
            }

            this.StepLengthMsec = clickLengthMsec;
            this.StepsPerBeat = clicksPerBeat;
            this.BeatsPerBar = beatsPerBar;
            this.LoopStartPositionSteps = loopStart;
            this.LoopEndPositionSteps = loopEnd;
        }

        public static OrganyaSong FromFile(string filePath)
        {
            using(var fileStream = File.OpenRead(filePath))
            {
                return OrganyaSong.FromStream(fileStream);
            }
        }

        public static OrganyaSong FromStream(Stream inputStream) 
        { 
            /*
                Org Header:
                [0x00]  char[6] Magic number: Always "Org-02", presumably a version number.
                [0x06]  short   Millisecond delay between beats. "Wait" param in OrgMaker.
                [0x08]  byte    Time signature. The "numerator" of the time signature. Does not affect playback.
                [0x09]  byte    Time Signature. The "denominator" of the time signature. Does not affect playback.
                [0x0A]  int32   Loop start. (Beats/Measure) * Measure. The beat that the loop starts on.
                [0x0E]  int32   Loop end. The beat that the song loops before. (Hard cap at 4080.)

                Instrument table: Repeat the following sixteen times.
                [0x12] Start of an instrument block. 
                
                [+0x00] short   The fine tuning parameter. 100-1900. "Voice" param in OrgMaker.
                [+0x02] byte    Instrument index. 0-99. This * 256 is the index in the wave lookup table.
                [+0x03] byte    Whether or not to play this instrument stacatto. Only affects instruments, not drums. This caps the instrument to a predetermined number of samples.
                [+0x04] ushort  Note Count. The amount of notes which use this instrument.

                Note table: Each instrument has a note table, placed sequentially one after the other.
                These are laid out in blocks by property. All of the starting beats for each note in a given instrument are grouped, 
                then the note values, then the length, et cetera.
                [0x72] Start address of note table. Repeat this once for each instrument.
                
                int     Beat Number- Beat which note event starts on. Not exactly sure why this is a int32.
                byte    Note- Represented as notes above c0. 0xFF signifies the last note's value should be used instead. (No change.)
                byte    length  How long this note is held in beats. 0xFF = No change
                byte    Volume- 0-254 Volume is not linear with 0 still being about 20%. 0xFF = No change
                byte    Pan- Pan doesn't work exactly like / pan. 0xFF = No Change.
             */

            using(BinaryReader reader = new BinaryReader(inputStream, System.Text.Encoding.ASCII))
            {
                //Header!
                var magicNum = reader.ReadChars(6);
                if (new String(magicNum) != "Org-02")
                {
                    throw new NotSupportedException(
                        String.Format("Unrecognized Organya header {0}: The file format isn't supported.", magicNum)
                    );
                }
                short msecDelay = reader.ReadInt16();
                byte timeSigNum = reader.ReadByte();
                byte timeSigDenom = reader.ReadByte();
                int loopStart = reader.ReadInt32();
                int loopEnd = reader.ReadInt32();

                var instrumentList = new List<OrganyaInstrument>();
                var noteCounts = new int[16];
                for(int j = 0 ; j < 16; j++)
                    
                {
                    //Read track.
                    short fineTune = reader.ReadInt16();
                    byte instrumentIndex = reader.ReadByte();
                    byte isPi = reader.ReadByte();
                    ushort noteCount = reader.ReadUInt16();
                    instrumentList.Add(new OrganyaInstrument() {
                        Notes = new List<OrganyaNote>(),
                        DisableSustain = isPi == 0? false : true,
                        FineTune = fineTune,
                        InstrumentIndex = instrumentIndex 
                    });
                    noteCounts[j] = noteCount;
                }

                byte prevVolume = 254;
                byte prevPan = 6;
                byte prevPitch = 96; //This one shouldn't be used ever.

                for (int i = 0; i < 16; i++) 
                {

                    #region Populate Notes...
                    var notes = new List<OrganyaNote>();
                    var noteCount = noteCounts[i];

                    List<int> notePositions = new List<int>();
                    List<byte> notePitches = new List<byte>();
                    List<byte> noteLengths = new List<byte>();
                    List<byte> noteVolumes = new List<byte>();
                    List<byte> notePans = new List<byte>();

                    for (int j = 0; j < noteCount; j++)
                    {
                        notePositions.Add(reader.ReadInt32());
                    }

                    for (int j = 0; j < noteCount; j++)
                    {
                        var pitch = reader.ReadByte();
                        if (pitch == 255) //255 == no change.
                        {
                            pitch = prevPitch;
                        }
                        else 
                        {
                            prevPitch = pitch;
                        }

                        notePitches.Add(pitch);
                    }

                    for (int j = 0; j < noteCount; j++)
                    {
                        noteLengths.Add(reader.ReadByte());
                    }
                    
                    for (int j = 0; j < noteCount; j++)
                    {
                        var volume = reader.ReadByte();
                        if (volume == 255) //255 == no change.
                        {
                            volume = prevVolume;
                        }
                        else 
                        {
                            prevVolume = volume;
                        }

                        noteVolumes.Add(volume);
                    }
                    
                    for (int j = 0; j < noteCount; j++)
                    {
                        var pan = reader.ReadByte();
                        if (pan == 255) //255 == no change.
                        {
                            pan = prevPan;
                        }
                        else 
                        {
                            prevPan = pan;
                        }

                        notePans.Add(pan);
                    }

                    for (int j = 0; j < noteCount; j++)
                    {
                        var pos = notePositions[j];
                        var pitch = notePitches[j];
                        var len = noteLengths[j];
                        var vol = noteVolumes[j];
                        var pan = notePans[j];

                        notes.Add(new OrganyaNote() {
                            BeatNumber = pos,
                            Pitch = pitch,
                            Length = len,
                            Volume = vol,
                            Pan = pan
                        });
                    }
                    #endregion

                    instrumentList[i].Notes = notes;
                }
                return new OrganyaSong(msecDelay, timeSigNum, timeSigDenom, loopStart, loopEnd, instrumentList);
            }
        }
    }
}

