using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils.UI;

namespace TopDownHordeShooter.Utils.GameState.States
{
    public class MenuState : BaseState
    {
        private readonly List<Button> _components;
        private int _difficultyLevel;

        private readonly SoundEffectInstance _menuSongInstance;
        private readonly ScoreManager _scoreManager;
        
        // UI text
        private readonly Text _uiText;
        
        public MenuState(HordeShooterGame game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
            var buttonTexture = Content.Load<Texture2D>("Graphics/Button");
            var font = Content.Load<SpriteFont>("Fonts/Arial");
            var menuSong = Content.Load<SoundEffect>("Sound/MenuTrack");
            _menuSongInstance = menuSong.CreateInstance(); 
            
            _difficultyLevel = 0; // Easy by default
            // Text
            _uiText = new Text (content);

            // Init score manager
            _scoreManager = ScoreManager.Load();
            
            // Create buttons 
            var newGameButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 3, 200),
                Text = "New Game",
            };
            
            var quitGameButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 3, 250),
                Text = "Quit Game",
            };

            var lowDiffButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 250),
                Text = "Easy",
            };
            
            var midDiffButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 300),
                Text = "Mid",
            };
            
            var hardDiffButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 350),
                Text = "Hard",
            };
            
            // Assign click events
            hardDiffButton.Click += (sender, args) => _difficultyLevel = 2;
            midDiffButton.Click += (sender, args) => _difficultyLevel = 1;
            lowDiffButton.Click += (sender, args) => _difficultyLevel = 0;
            newGameButton.Click += NewGameButton_Click;
            quitGameButton.Click += QuitGameButton_Click;
            
            _components = new List<Button>
            {
                newGameButton,
                quitGameButton,
                lowDiffButton,
                midDiffButton,
                hardDiffButton
            };

            _menuSongInstance.Play();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Gray);
            foreach (var component in _components)
                component.Draw(spriteBatch);

            _uiText.DrawString("High scores:\n" + string.Join("\n", _scoreManager.Highscores.Select(c => c.Value)),
                new Vector2(30, 40), Color.White, 1, false, spriteBatch);

            string diffText;
            switch (_difficultyLevel)
            {
                case 0:
                    diffText = "Easy";
                    break;
                case 1:
                    diffText = "Medium";
                    break;
                case 2:
                    diffText = "Hard";
                    break;
                default:
                    diffText = "N/A";
                    break;
            }
            _uiText.DrawString("Difficulty level:" + diffText,
                new Vector2(_components[2].Position.X, _components[2].Position.Y - 35), Color.White, 1, false, spriteBatch);
            
            if(Game.LostGame)
                _uiText.DrawString("Game Lost. New game?", new Vector2(GraphicsDevice.Viewport.Width / 2, 40), Color.DarkRed, 1, false, spriteBatch);
            ShowInstructions(spriteBatch);
            
            spriteBatch.End();
        }

        private void ShowInstructions(SpriteBatch spriteBatch)
        {
            const string instructions = "Instructions: WASD to move, E/Q to change weapons, which unlock as you level up.";
            _uiText.DrawString(instructions, new Vector2(GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 2 + 100), Color.White, 1.5f, false, spriteBatch);
        }
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            Game.LostGame = false;
            Game.DifficultyLevel = _difficultyLevel;
            _menuSongInstance.Stop();
            Game.ChangeState(new GameState(Game, GraphicsDevice, Content));
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

        private void QuitGameButton_Click(object sender, EventArgs e) => Game.Exit(); 
    }
}