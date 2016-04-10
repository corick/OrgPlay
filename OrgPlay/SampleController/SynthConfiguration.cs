using System;

using Microsoft.Xna.Framework.Audio;

namespace OrgPlay.SampleController
{
    public class SynthConfiguration
    {
        public readonly int SampleRate;
        public readonly AudioChannels Channels;

        public SynthConfiguration(int sampleRate = 44100, TimeSpan bufferLength = TimeSpan.FromMilliseconds(30), AudioChannels channels = AudioChannels.Stereo)
        {
            SampleRate = sampleRate;
            Channels = channels;
        }
    }
}

