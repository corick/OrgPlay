using System;
using System.Collections.Generic;

using OrgPlay.Organya;
using Microsoft.Xna.Framework;


namespace OrgPlay.Player
{
    public class OrganyaSongPlayer
        : ISourceSampleProvider
    {
        private readonly List<NoteChannelController> _noteChannels;

        private readonly OrganyaPlayerContext _context;

        public OrganyaSongPlayer(SampleProviderConfiguration config, OrganyaSong song, 
            WavetableSampleLoader sampleLoader)
        {
            _context = new OrganyaPlayerContext(config, song, sampleLoader);
            _noteChannels = new List<NoteChannelController>();

            for (int i = 0; i < song.Tracks.Count; i++)
            {
                var track = song.Tracks[i];
                bool isDrum = i >= 8;
                var channel = new NoteChannelController(_context, track, isDrum);
                _noteChannels.Add(channel);
            }
        }

        public float[] RequestBuffer(long lengthSamples)
        {
            float[] masterBuffer = new float[lengthSamples];
            //FIXME: Mixing: are tracks additive?
            for (int trackIndex = 0; trackIndex < _noteChannels.Count; trackIndex++)
            {
                var buf = _noteChannels[trackIndex].RequestBuffer(_context, lengthSamples);

                //Not sure how this is supposed to be handled, but I think that clamping them afterwards would sound better.
                for (int i = 0; i < buf.Length; i++)
                {
                    masterBuffer[i] += buf[i];
                }
            }

            for (int i = 0; i < masterBuffer.Length; i++)
            {
                masterBuffer[i] = MathHelper.Clamp(masterBuffer[i], -1, 1);
            }

            return masterBuffer;
        }
    }
}

