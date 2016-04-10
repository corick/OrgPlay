using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace OrgPlay
{
	public class PlayerGame
		: Microsoft.Xna.Framework.Game
	{
		private readonly GraphicsDeviceManager _gfx;
        private readonly DynamicSoundEffectInstance _dsi;
       
		public PlayerGame()
		{
			_gfx = new GraphicsDeviceManager (this);
			_gfx.PreferredBackBufferWidth = 800;
			_gfx.PreferredBackBufferHeight = 600;
		}

        protected override void Initialize()
        {
        }

        protected override void Update(GameTime gameTime)
        {
        }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
		}
	}
}

