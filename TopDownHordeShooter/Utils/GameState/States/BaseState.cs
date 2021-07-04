using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils.GameState.States
{
    public abstract class BaseState
    {
        #region Fields

        protected ContentManager Content;

        protected GraphicsDevice GraphicsDevice;

        protected HordeShooterGame Game;

        #endregion

        #region Methods

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate(GameTime gameTime);

        public BaseState(HordeShooterGame game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            Game = game;

            GraphicsDevice = graphicsDevice;

            Content = content;
        }

        public abstract void Update(GameTime gameTime);

        #endregion
    }
}