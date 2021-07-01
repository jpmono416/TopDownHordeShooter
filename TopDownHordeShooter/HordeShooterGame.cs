using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils.GameState.States;

namespace TopDownHordeShooter
{
    public class HordeShooterGame : Game
    {
        #region Properties and fields
        private readonly GraphicsDeviceManager _graphics;

        public SpriteBatch SpriteBatch { get; private set; }

        // Game states
        private BaseState _currentState;
        private BaseState _nextState;

        public void ChangeState(BaseState state)
        {
            _nextState = state;
        }
        
        // Screen dimensions
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        #endregion

        #region Constructor
        public HordeShooterGame()
        {
            //Graphics
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        #endregion
        

        #region Initialize
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            
            _graphics.ApplyChanges();
            
            //Input

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Game state
            _currentState = new MenuState(this, GraphicsDevice, Content);
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }
            // Game state
            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);
            
            base.Update(gameTime);
        }
        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Game state
            _currentState.Draw(gameTime, SpriteBatch);


            base.Draw(gameTime);
        }
        #endregion
    }
    
    #region Main
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new HordeShooterGame();
            game.Run();
        }
    }
    #endregion
}
