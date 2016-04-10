using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;


namespace OrgPlay.Organya
{
    public class OrganyaSong
    {
        public readonly ImmutableList<Track> Tracks;

        public readonly int ClickLengthMsec;
        public readonly int ClicksPerBeat;
        public readonly int LoopStartPositionClicks;
        public readonly int LoopEndPositionClicks;

        public OrganyaSong(int clickLengthMsec, int clicksPerBeat, int loopStart, int loopEnd, IEnumerable<Track> tracks)
        {
            Tracks = tracks.ToImmutableList();

            if (Tracks.Count != 6)
            {
                throw new ArgumentException("Received an unexpected track count.");
            }

            this.ClickLengthMsec = clickLengthMsec;
            this.ClicksPerBeat = clicksPerBeat;
            this.LoopStartPositionClicks = loopStart;
            this.LoopEndPositionClicks = loopEnd;
        }

        public static OrganyaSong FromFile(string filePath)
        {
            return OrganyaSong.FromStream(filePath);
        }

        public static OrganyaSong FromStream(Stream inputStream) 
        { 
            /*
                Org Header:
                [0x00]  char[6] Magic number: Always "Org-02", presumably a version number.
                [0x06]  short   Millisecond delay between beats. "Wait" param in OrgMaker.
                [0x08]  byte    Time signature. The "numerator" of the time signature. Does not affect playback.
                [0x09]  byte    Time Signature. The "denominator" of the time signature. Does not affect playback.
                [0x0A]  short?  Loop start. (Beats/Measure) * Measure. The beat that the loop starts on.
                [0x0C]  short?  Loop end. The beat that the song loops before. (Hard cap at 4080.)
                [0x0E]  int32   Not Used?

                Frequency table: Repeat the following six times.
                [0x12] Start Addr
                [+0x00] short   The fine tuning parameter. 100-1900. "Voice" param in OrgMaker.
                [+0x02] byte    Instrument index. 0-99. This * 256 is the index in the wave lookup table.
                [+0x03] byte    Whether or not to play this instrument stacatto. Only affects instruments, not drums. This caps the instrument to a predetermined number of samples.
                [+0x04] ushort  Note Count. The amount of notes which use this instrument.

                Note table. 
                Each instrument has a note table, placed sequentially one after the other.
                [0x72] Start address of note table. Repeat this once for each instrument.
                int     Beat Number- Beat which note event starts on. Not exactly sure why this is a int32.
                byte    Note- Represented as notes above c0
                byte    length  How long this note is held in beats.
                byte    Volume- 0-255 Volume is not linear with 0 still being about 20%
                byte    Pan- Pan works in a weird way too.
                Notes are interleaved by property. 
                (e.g. for an instrument with four notes the table might look like:
                bbbbnnnnllllvvvvpppp).
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

                for (int i = 0; i < 6; i++)
                {
                    //Read track.
                    short fineTune = reader.ReadInt16();
                    byte instrumentIndex = reader.ReadByte();
                    byte isPi = reader.ReadByte();
                    ushort noteCount = reader.ReadUInt16();

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
                        notePitches.Add(reader.ReadByte());
                    }

                    for (int j = 0; j < noteCount; j++)
                    {
                        noteLengths.Add(reader.ReadByte());
                    }
                    
                    for (int j = 0; j < noteCount; j++)
                    {
                        noteVolumes.Add(reader.ReadByte());
                    }
                    
                    for (int j = 0; j < noteCount; j++)
                    {
                        notePans.Add(reader.ReadByte());
                    }

                    for (int j = 0; j < noteCount; j++)
                    {
                        // TODO: Create a new Note object, add it to the note list for the track.
                    }

                }

            }
        }
    }
}

