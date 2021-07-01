using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Misc;
using TopDownHordeShooter.Utils.UI;

namespace TopDownHordeShooter.Utils.GameState.States
{
    public class MenuState : BaseState
    {
        private List<Component> _components;
        private int DifficultyLevel;

        private ScoreManager _scoreManager;
        
        // UI text
        private Text uiText;
        
        public MenuState(HordeShooterGame game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
            var buttonTexture = _content.Load<Texture2D>("Graphics/Button");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Arial");
            
            DifficultyLevel = 0; // Easy by default
            // Text
            uiText = new Text (content);

            // Init score manager
            _scoreManager = ScoreManager.Load();
            
            // Create buttons 
            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 200),
                Text = "New Game",
            };
            
            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 250),
                Text = "Load Game",
            };

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 300),
                Text = "Quit Game",
            };

            var lowDiffButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(600, 250),
                Text = "Easy",
            };
            
            var midDiffButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(600, 300),
                Text = "Mid",
            };
            
            var hardDiffButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(600, 350),
                Text = "Hard",
            };
            
            // Assign click events
            hardDiffButton.Click += (sender, args) => DifficultyLevel = 2;
            midDiffButton.Click += (sender, args) => DifficultyLevel = 1;
            lowDiffButton.Click += (sender, args) => DifficultyLevel = 0;
            newGameButton.Click += NewGameButton_Click;
            loadGameButton.Click += LoadGameButton_Click;
            quitGameButton.Click += QuitGameButton_Click;
            
            _components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                quitGameButton,
                lowDiffButton,
                midDiffButton,
                hardDiffButton
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            uiText.DrawString("High scores:\n" + string.Join("\n", _scoreManager.Highscores.Select(c => c.Value)),
                new Vector2(30, 40), Color.White, 1, false, spriteBatch);
            
            spriteBatch.End();
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Load Game");
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }
    }
}