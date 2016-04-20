using System;
using OrgPlay.Organya;

namespace OrgPlay.Player
{
    /// <summary>
    /// Sample provider that plays a discrete note event.
    /// </summary>
    public class NoteEventController
    {

        private readonly static int POINT_FREQ_RELATIVE_TO_SAMPLE_FREQ = 44100;
        private readonly static int[] NOTE_POINT_FREQS = new int[] {  //FIXME: Derive these instead.
            33408, //C
            35584, //C#
            37632, //D
            39808, //D#
            42112, //E
            44672, //F
            47488, //F#
            50048, //G
            52992, //G#
            56320, //A
            59648, //A#
            63232  //B
        };

        private readonly static int KEYS_IN_OCTAVE = 12;
        private readonly static int[] OCTAVE_REPEAT_TIMES = new int[]   { 4, 2, 2, 2, 2, 2, 2, 2  };
        private readonly static int[] OCTAVE_ADVANCE_POINTS = new int[] { 1, 1, 2, 4, 8, 16, 32, 64 };

        private readonly WavetableSample _sample;
        private readonly float _normalizedSampleSpeed;
        private readonly int _octave, _lengthSamples;
        private readonly float _volumeNormalized;
        private readonly float _panNormalized;

        private float samplePosition;
        private int samplesPlayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgPlay.SampleController.WavetableNoteSampleProvider"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="note">Note.</param>
        /// <param name="bankNumber">Bank number.</param>
        /// <param name="pitchBendNormalized">Pitch bend normalized.</param>
        /// <param name="drum">If set to <c>true</c> drum.</param>
        /// <param name="pi">If set to <c>true</c> pi.</param>
        public NoteEventController(OrganyaPlayerContext context, OrganyaNote note, int bankNumber, 
            int pitchBendNormalized, bool drum, bool pi)
        {
            _sample = context.SampleLoader.Load(bankNumber);
            samplePosition = 0;
            samplesPlayed = 0;

            var pitchIndex = note.Pitch % KEYS_IN_OCTAVE;
            _octave = note.Pitch / KEYS_IN_OCTAVE;

            _volumeNormalized = (float)Math.Pow(10, ((float)note.Volume / 254) - 1);
            _panNormalized = ((float)(note.Pan - 6)) / 6.0f; 

            var samplePointFrequency = NOTE_POINT_FREQS[pitchIndex] + pitchBendNormalized;
            _normalizedSampleSpeed = samplePointFrequency / ((float)POINT_FREQ_RELATIVE_TO_SAMPLE_FREQ);

            if(drum)
            {
                //TODO: Not sure how drums are affected by pitch.

                //_normalizedSampleSpeed = 1.0f; //Do drums play at a constant point frequency? 
                float stretchFactor = ((float)OCTAVE_REPEAT_TIMES[_octave]) / ((float)OCTAVE_ADVANCE_POINTS[_octave]);
                _lengthSamples = (int) (((float)_sample.Samples.Length / _normalizedSampleSpeed) * stretchFactor);
            }
            else if(pi)
            {
                //Is this correct? It should play a constant amount of samples I think.
                _lengthSamples = 256;
                //TODO: Lengths are given as periods in the notes page explanation of the 'pi' option. 
            }
            else
            {
                //In this case, we know how long this'll have to be in samples.
                _lengthSamples = Utilities.StepsToSamples(note.Length, context.StepMsec, context.Config.SampleRate);
            }
        }

        //FIXME: This should be stateless. 

        /// <summary>
        /// Requests an individual sample from the note.
        /// </summary>
        /// <param name="left">Return value of the left channel.</param>
        /// <param name="right">Return value of ther right channel.</param>
        public void RequestSample(out float left, out float right)
        {
            if(this.samplesPlayed > this._lengthSamples)
            {
                left = right = 0;
                return;
            }

            //Take the position in the intermediate waveform
            //Divide that by the number of times the samples repeat
            //Multiply by the amount of points the sample has to advance.
            //This gets the index in the intermediate (octave-adjusted) waveform
            int absoluteSampleIndex = (((int)Math.Floor(samplePosition)) / OCTAVE_REPEAT_TIMES[_octave]) * OCTAVE_ADVANCE_POINTS[_octave];
            int wrappedSampleIndex = absoluteSampleIndex % _sample.Samples.Length;

            Utilities.SplitChannel(_sample.Samples[wrappedSampleIndex], _panNormalized, out left, out right);
            left = left * _volumeNormalized;
            right = right * _volumeNormalized;

            samplePosition += _normalizedSampleSpeed;
            samplesPlayed++;
        }

        /// <summary>
        /// Resets the state of this note provider, so it can be replayed. (Used for looping.)
        /// </summary>
        public void Reset()
        {
            //Reset the state so we can replay the note.
            samplePosition = samplesPlayed = 0;
        }
    }
}

