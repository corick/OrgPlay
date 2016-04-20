using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

using OrgPlay.Player;

namespace OrgPlay
{
    public class SampleControllerComponent
        : Microsoft.Xna.Framework.GameComponent
    {
        private readonly DynamicSoundEffectInstance _dsi;
        private readonly SampleProviderConfiguration _config;
        private readonly ISourceSampleProvider _master;

        public SampleControllerComponent(Game game, ISourceSampleProvider sourceSampleProvider, SampleProviderConfiguration config)
            : base(game)
        {
            _config = config;
            _master = sourceSampleProvider;

            _dsi = new DynamicSoundEffectInstance (config.SampleRate, config.Channels);
            _dsi.Play();

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            while (_dsi.PendingBufferCount < 3)
            {
                var buf = _master.RequestBuffer(_config.BufferLengthSamples);
                _dsi.SubmitFloatBufferEXT(buf);
            }

            base.Update(gameTime);
        }

    }
}

