using System;
using Microsoft.Xna.Framework.Audio;

namespace OrgPlay.SampleController
{
    public class SampleControllerComponent
        : Microsoft.Xna.Framework.GameComponent
    {
        private readonly DynamicSoundEffectInstance _dsi;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgPlay.SynthContext"/> class.
        /// </summary>
        /// <param name="sourceSampleProvider">The root source sample provider to .</param>
        public SampleControllerComponent(ISourceSampleProvider sourceSampleProvider, SynthConfiguration config)
        {
            _dsi = new DynamicSoundEffectInstance (config.SampleRate, config.Channels);
            _dsi.Play();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
}

