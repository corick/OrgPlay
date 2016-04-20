using System;
using System.Collections.Generic;
using System.Linq;
using OrgPlay.Organya;


namespace OrgPlay.Player
{
    public class NoteChannelController
    {
        //Container so we can have these sorted on beat and look up the current note without the 
        //note provider actually knowing when it starts / stops.
        private class NoteEvent 
        {
            public NoteEventController Note { get; set; }
            public int StepOffset { get; set; }
            public int LengthSteps { get; set; }
        }

        private static readonly int PITCH_BEND_NORMALIZATION_FACTOR = 1000;

        private readonly List<NoteEvent> _notes; 

        private int currentNoteIndex;
        private int currentSampleIndex;

        public NoteChannelController(OrganyaPlayerContext context, OrganyaInstrument instrument, bool isDrum)
        {
            currentNoteIndex = 0;
            currentSampleIndex = 0;
            var pitchBend = instrument.FineTune - PITCH_BEND_NORMALIZATION_FACTOR;

            var tempNotes = new List<NoteEvent>();

            foreach(var note in instrument.Notes)
            {
                var adjustedInstrumentIndex = isDrum? instrument.InstrumentIndex + 100 : instrument.InstrumentIndex;
                NoteEventController prov = new NoteEventController(context, note, adjustedInstrumentIndex ,
                                                       pitchBend, isDrum, instrument.DisableSustain);
 
                tempNotes.Add(new NoteEvent() {
                    Note = prov,
                    StepOffset = note.BeatNumber,
                    LengthSteps = note.Length
                }
                );
            }

            _notes = tempNotes.OrderBy(
                (ne) => ne.StepOffset
            ).ToList();
        }

        //TODO: Add reset() for looping.

        public float[] RequestBuffer(OrganyaPlayerContext context, long lengthSamples)
        {
            float[] sampleBuffer = new float[lengthSamples];

            if(_notes.Count == 0) //Skip if there's no notes to play.
            {
                return sampleBuffer;
            }

            float l, r;
            for(int i = 0; i < lengthSamples; i+= 2)
            {
                //Bounds checking - make sure that we don't crash the program at the end of the song.
                if(this.currentNoteIndex >= _notes.Count)
                    break;
                
                var activeNote = _notes[this.currentNoteIndex];
                var currentBeat = Utilities.SamplesToSteps(this.currentSampleIndex, context.StepMsec, context.Config.SampleRate);

                //Check if the other note is overlapping this note. 
                //When two notes overlap (common - comment this out and listen to Jenka 1), the second note should always play.
                //TODO: Figure out if this is necessary, or only maskign a bug.
                if(this.currentNoteIndex + 1 < _notes.Count && currentBeat == _notes[currentNoteIndex + 1].StepOffset)
                {
                    activeNote = _notes[this.currentNoteIndex + 1];
                    currentNoteIndex++;
                }

                if(currentBeat > activeNote.StepOffset + activeNote.LengthSteps) 
                {
                    //The note is done playing. Advance to the last note unless this is the last one.
                    currentNoteIndex++;

                    if(currentNoteIndex < _notes.Count)
                    {
                        activeNote = _notes[currentNoteIndex];
                    }
                }

                if(currentBeat >= activeNote.StepOffset)
                {
                    //We've advanced to the correct note at this point and it's active, so play it.
                    activeNote.Note.RequestSample(out l, out r);
                    sampleBuffer[i] = l;
                    sampleBuffer[i + 1] = r;
                }
                else
                {
                    sampleBuffer[i] = sampleBuffer[i + 1] = 0;
                }

                currentSampleIndex++;
            }
            return sampleBuffer;
        }
    }
}

