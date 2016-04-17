using System;

using Microsoft.Xna.Framework.Audio;

namespace OrgPlay.SampleController
{
    public class SampleProviderConfiguration
    {
        public readonly int SampleRate;
        public readonly AudioChannels Channels;
        public readonly int BufferLengthSamples;

        public SampleProviderConfiguration(int sampleRate, TimeSpan bufferLength, AudioChannels channels)
        {
            SampleRate = sampleRate;
            Channels = channels;
            BufferLengthSamples = Utilities.TimeSpanToSamples(bufferLength, sampleRate) * 2; //One for each channel.
        }
    }
}

