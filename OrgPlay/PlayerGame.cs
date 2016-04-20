using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using OrgPlay.Player;

namespace OrgPlay
{
	public class PlayerGame
		: Microsoft.Xna.Framework.Game
	{
		private readonly GraphicsDeviceManager _gfx;
        private readonly string _songName;
        private SampleControllerComponent synth;
       
        public PlayerGame(string songName)
		{
            _songName = songName;
			_gfx = new GraphicsDeviceManager (this);
			_gfx.PreferredBackBufferWidth = 800;
			_gfx.PreferredBackBufferHeight = 600;
		}

        protected override void LoadContent()
        {
            var loader = new WavetableSampleLoader();
            //var sampleProvider = new WavetableNoteSampleProvider(loader.Load(102), 40, 254, 0, 9999, 1000, true, false);
            var song = Organya.OrganyaSong.FromFile(_songName);
            var config = new SampleProviderConfiguration(44100, TimeSpan.FromMilliseconds(60), AudioChannels.Stereo);
            var sampleProivder = new OrganyaSongPlayer(config, song, new WavetableSampleLoader());


            synth = new SampleControllerComponent(
                this,
                sampleProivder,
                config
            );

            Components.Add(synth);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
		}
	}
}

