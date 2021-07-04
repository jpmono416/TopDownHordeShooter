using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownHordeShooter.Entities.Characters;
using TopDownHordeShooter.Entities.Combat.Weapons;
using TopDownHordeShooter.Entities.Misc;
using TopDownHordeShooter.Entities.Pickups;
using TopDownHordeShooter.Utils.Events;
using TopDownHordeShooter.Utils.UI;

namespace TopDownHordeShooter.Utils.GameState.States
{
    public class GameState : BaseState
    {
        private readonly ContentManager _content;
        private readonly ScoreManager _scoreManager;
        private SpriteBatch _spriteBatch;
        
        // Background
        private Texture2D _backgroundTexture;
        private readonly BackgroundMapTiles _background;
        
        // Enemies
        private readonly List<Texture2D> _walkerEnemySpriteSheets;
        private readonly List<Texture2D> _shooterEnemySpriteSheets;
        private readonly List<Enemy> _enemies;

        // Player
        private readonly Player _player;
        private readonly List<Texture2D> _playerSpriteSheets;

        // Weapons
        private Texture2D _weaponsSpritesheet;
        
        // Projectiles
        private Texture2D _redProjectilesSpritesheet;
        private Texture2D _blueProjectilesSpritesheet;

        // Pickups
        private Texture2D _pickupsSpritesheet;
        private List<BasePickup> _pickups;

        private readonly GraphicsDevice _graphicsDevice;
        
        // Input
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState CurrentMouseState { get; set; }
        
        // UI
        private Text _uiText;
        private List<Component> _pauseButtons;
        
        // Song
        private SoundEffect _gameSong;
        private SoundEffectInstance _gameSongInstance;
        
        // Game data
        private readonly GameData _gameData;

        // Enemy waves manager
        private readonly WaveManager _waveManager;

        private bool _gamePaused;

        public GameState(HordeShooterGame game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
            _graphicsDevice = graphicsDevice;
            _content = content;
            _spriteBatch = game.SpriteBatch;
            _gameData = InitializeGameData();

            _background = new BackgroundMapTiles();
            
            _player = new Player(_gameData.PlayerStats);
            _playerSpriteSheets = new List<Texture2D>();
            
            _enemies = new List<Enemy>();
            _walkerEnemySpriteSheets = new List<Texture2D>();
            _shooterEnemySpriteSheets= new List<Texture2D>();
            

            _content.RootDirectory = "Content";
            _waveManager = new WaveManager();
            _scoreManager = ScoreManager.Load();
            
            LoadContent();
        }

        private void LoadContent()
        {
            _backgroundTexture = _content.Load<Texture2D>("Graphics\\stone_floor");

            // UI
            _uiText = new Text(_content);
            CreatePauseButtons();

            
            
            // Player
            var playerSpawnPosition = Vector2.Zero;
            //new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);


            // Player textures and weapons inventory
            _playerSpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Player_idle"));
            _playerSpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Player_hurt"));
            _playerSpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Player_run"));
            _playerSpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Player_death"));

            _weaponsSpritesheet = _content.Load<Texture2D>("Graphics\\Weapons");
            
            // Song
            _gameSong = Content.Load<SoundEffect>("Sound\\TooCrazy");
            _gameSongInstance = _gameSong.CreateInstance();
            
            // Weapons sounds
            var tempBerettaSound = Content.Load<SoundEffect>("Sound\\PistolShot");
            var tempAssaultRifleSound = Content.Load<SoundEffect>("Sound\\RifleShot");
            var tempSniperSound = Content.Load<SoundEffect>("Sound\\SniperShot");
            var tempFlamethrowerSound = Content.Load<SoundEffect>("Sound\\Flamethrower");
            
            var tempList = new List<SoundEffect>
                { tempBerettaSound, tempAssaultRifleSound, tempSniperSound, tempFlamethrowerSound };

            _player.Weapons = BaseWeapon.InitAllWeapons(_weaponsSpritesheet, tempList);
            _player.Initialize(_playerSpriteSheets, playerSpawnPosition, 48);
            
            // Projectiles
            _redProjectilesSpritesheet = _content.Load<Texture2D>("Graphics\\RedProjectilesAnimation");
            _blueProjectilesSpritesheet = _content.Load<Texture2D>("Graphics\\BlueProjectilesAnimation");
            
            // Pickups
            _pickupsSpritesheet = _content.Load<Texture2D>("Graphics\\Pickups");
            _pickups = BasePickup.InitAllPickups(_pickupsSpritesheet, _gameData.DifficultyData.BasePickupSpawnTime);

            // Walker enemy texture
            _walkerEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Zombie-Walking-Animation"));
            _walkerEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Zombie-Walking-Animation"));
            _walkerEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Zombie-Damage-Animation"));
            _walkerEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\Zombie-Dying-Animation"));
            
            // Shooter enemy texture
            _shooterEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\ShooterEnemy_idle"));
            _shooterEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\ShooterEnemy_run"));
            _shooterEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\ShooterEnemy_hurt"));
            _shooterEnemySpriteSheets.Add(_content.Load<Texture2D>("Graphics\\ShooterEnemy_kill"));
            

            // Enemies
            SpawnEnemies();

            _background.Initialize(_backgroundTexture); 
        }
        
        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            // Input
            UpdateInputMethods();
            CheckInput(gameTime);

            if (_gamePaused)
            {
                foreach (var button in _pauseButtons) button.Update(gameTime); 
                return;
            }

            // Player
            _player.Update(gameTime);


            // Enemies
            var tempDeleteEnemies = new List<Enemy>();
            foreach (var enemy in _enemies)
            {
                // Remove dead enemies from list
                if (!enemy.Active)
                {
                    tempDeleteEnemies.Add(enemy);
                    continue;
                }
                enemy.Update(gameTime);
            }

            foreach (var toBeRemoved in tempDeleteEnemies)
            {
                // Give XP to the player
                _player.PlayerController.GainXp(new PlayerEventArgs(PlayerEventType.GainXp, gameTime, _blueProjectilesSpritesheet, toBeRemoved.ScoreGiven));
                _enemies.Remove(toBeRemoved);
            }
            
            
            // Pickups
            foreach (var pickup in _pickups)
                pickup.Update(gameTime);

            CalculateCollisions(gameTime);
            
            // Update game loop
            if (_enemies.Count == 0) _waveManager.WaveCompleted = true;
            _waveManager.Update(gameTime);
            if (_waveManager.TimerOver)
            {
                _waveManager.TimerOver = false;
                SpawnEnemies();
            }
            
            CheckSong();
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin();

            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background
            _background.Draw(spriteBatch);
            
            // Player
            _player.Draw(spriteBatch);

            // Enemies
            foreach (var enemy in _enemies) enemy.Draw(spriteBatch);

            // Pickups
            foreach (var pickup in _pickups) pickup.Draw(spriteBatch);
            
            // UI
            DrawUi(spriteBatch);
            
            spriteBatch.End();
        }

        private GameData InitializeGameData()
        {
            var enemyDataString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\EnemyData.json");
            var playerStatsString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\PlayerStats.json");
            var difficultyDataString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\DifficultyData.json");

            var enemyDataObj = (List<EnemyData>) FileLoader.JsonToObject(enemyDataString, typeof(List<EnemyData>));
            var playerStatsSObj = (PlayerStats) FileLoader.JsonToObject(playerStatsString, typeof(PlayerStats));
            var difficultyDataObj =
                (List<DifficultyData>) FileLoader.JsonToObject(difficultyDataString, typeof(List<DifficultyData>));

            return new GameData(playerStatsSObj, enemyDataObj, difficultyDataObj[Game.DifficultyLevel]);
        }
        
        private void CreatePauseButtons()
        {
            var buttonTexture = Content.Load<Texture2D>("Graphics/Button");
            var buttonFont = Content.Load<SpriteFont>("Fonts/Arial");
            
            var returnToGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 200),
                Text = "Return to game",
            };
            
            var abandonButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(GraphicsDevice.Viewport.Width / 2, 250),
                Text = "Abandon",
            };

            returnToGameButton.Click += (sender, args) => _gamePaused = !_gamePaused;
            abandonButton.Click += (sender, args) => EndGame();

            _pauseButtons = new List<Component>
            {
                returnToGameButton,
                abandonButton
            };
        }

        private void CheckSong()
        {
            if(_gameSongInstance.State != SoundState.Playing) _gameSongInstance.Play();
        }
        private void DrawUi(SpriteBatch spriteBatch)
        {
            if (_gamePaused)
            {
                foreach (var button in _pauseButtons) button.Draw(spriteBatch);
                return;
            }
            
            _uiText.DrawString("Level: " + _player.Level,new Vector2(_graphicsDevice.Viewport.Width-150,20),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Exp: " + _player.XpPoints + " / " + _player.NextLevelXpRequired,new Vector2(_graphicsDevice.Viewport.Width-200,50),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Health: " + _player.Health,new Vector2(20,20),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Rate of fire: " + ( 1000 * 60 / _player.RateOfFire.Milliseconds) + " bpm",new Vector2(20,50),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Damage: " + _player.BaseDamage,new Vector2(20,80),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Ammo: " + _player.CurrentWeapon.CurrentBulletsInMag + " / " + _player.CurrentWeapon.RemainingBullets,new Vector2(20,110),Color.White,2,false, spriteBatch);
            _uiText.DrawString("Current wave: " + _waveManager.CurrentWave,new Vector2(20,_graphicsDevice.Viewport.Height - 40),Color.Crimson,2,false, spriteBatch);

            // Display health counters
            foreach (var enemy in _enemies)
                _uiText.DrawString(enemy.Health.ToString(),new Vector2(enemy.CharacterAnimation.Position.X + enemy.CharacterAnimation.FrameWidth / 2, enemy.CharacterAnimation.Position.Y - enemy.CharacterAnimation.FrameHeight / 2),Color.Firebrick,1,false, spriteBatch);
        }
        
        #region Game Actions
        private void SpawnEnemies()
        {
            // Spawn walker enemies
            for (var i = 0; i <= _gameData.DifficultyData.MinimumEnemiesPerWave + _waveManager.CurrentWave * _gameData.DifficultyData.EnemiesPerWaveMultiplier; i++)
            {
                var r = new Random();
                var spawnChance = r.Next(0, 100);
                foreach (var enemyData in _gameData.EnemyData.Where(ed => spawnChance <= ed.SpawnRate))
                {
                    var enemyPosition = new Vector2(r.Next(0, _graphicsDevice.Viewport.Width),
                        r.Next(0, _graphicsDevice.Viewport.Height)); 
                    
                    switch (enemyData.Type)
                    {
                        case 0:
                            var tempWalker = new WalkerEnemy(enemyData, _gameData.DifficultyData);
                            tempWalker.Initialize(_walkerEnemySpriteSheets, enemyPosition, 64);
                            _enemies.Add(tempWalker);
                            break;
                        case 1:
                            if (enemyData.MinimumWave > _waveManager.CurrentWave)
                            {
                                --i; // Void entry and repeat with another value
                                break;
                            }
                            var tempShooter = new ShooterEnemy(enemyData, _gameData.DifficultyData, _player, _redProjectilesSpritesheet);
                            tempShooter.Initialize(_shooterEnemySpriteSheets, enemyPosition, 32);
                            _enemies.Add(tempShooter);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

       
        private void CalculateCollisions(GameTime gameTime)
        {
            _player.Colliding = false;

            // Enemies
            foreach (var enemy in _enemies)
            {
                // Update player position
                enemy.PlayerPosition = _player.Position;

                // Check player collisions
                if (_player.Hitbox.Collides(enemy.Hitbox))
                {
                    _player.Colliding = true;
                    _player.CollidingWith = ColliderType.Enemy;
                    _player.TakeDamage(enemy.BaseDamage, enemy.Hitbox.ColliderType, gameTime);
                    
                    // If player is dead terminate game
                    if(!_player.Active) EndGame();
                }

                // Calculate projectile collisions
                

                if (!(enemy is ShooterEnemy shooterEnemy)) continue;
                
                foreach (var proj in shooterEnemy.ActiveProjectiles.Where(proj => proj.Hitbox.Collides(_player.Hitbox)))
                {
                    _player.Colliding = true;
                    _player.CollidingWith = ColliderType.EnemyProjectile;
                    _player.TakeDamage(proj.Damage, proj.Hitbox.ColliderType, gameTime);
                    
                    // If player is dead terminate game
                    if(!_player.Active) EndGame();
                }
            }

            // Logic separated as it seemed to glitch when inside the other loop around enemies
            foreach (var projectile in _player.CurrentWeapon.ActiveProjectiles)
            {
                foreach (var enemy in _enemies.Where(enemy => enemy.Hitbox.Collides(projectile.Hitbox)))
                {
                    // Inflict damage to enemy
                    enemy.Colliding = true;
                    enemy.CollidingWith = ColliderType.Projectile;
                    enemy.TakeDamage(projectile.Damage + _player.CurrentWeapon.DamageOffset, projectile.Hitbox.ColliderType, gameTime);
                    
                    // Deactivate projectile
                    projectile.Active = false;
                    projectile.Hitbox.ColliderType = ColliderType.None;
                }
            }
            
            foreach (var pickup in _pickups.Where(pickup => pickup.Active).Where(pickup => pickup.Hitbox.Collides(_player.Hitbox)))
            {
                pickup.PickupController.ApplyEffect(new PickupEventArgs(PickupEventType.ApplyEffect, gameTime, _player));
                
                pickup.Active = false;
                pickup.LastGone = gameTime.TotalGameTime;
            }
        }

        private void EndGame()
        {
            // Prevent Update being executed multiple times and duplicating scores
            if (Game.LostGame) return;
            Game.LostGame = true;

            _scoreManager.Add(new Score{Value = _waveManager.CurrentWave});
            ScoreManager.Save(_scoreManager);

            _gameSongInstance.Stop();
            //_gameSongInstance.Dispose();
            //_gameSong.Dispose();
            Game.ChangeState(new MenuState(Game, GraphicsDevice, Content));
        }
        
        #endregion
        #region Input
        private void CheckInput(GameTime gameTime)
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape) && !PrevKeyboardState.IsKeyDown(Keys.Escape)) _gamePaused = !_gamePaused;
            if (_gamePaused) return;
            
            if (CurrentKeyboardState.IsKeyDown(Keys.A)) _player.PlayerController.MoveLeft(new PlayerEventArgs(PlayerEventType.MoveLeft, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.W)) _player.PlayerController.MoveUp(new PlayerEventArgs(PlayerEventType.MoveUp, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.S)) _player.PlayerController.MoveDown(new PlayerEventArgs(PlayerEventType.MoveDown, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.D)) _player.PlayerController.MoveRight(new PlayerEventArgs(PlayerEventType.MoveRight, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.R)) _player.PlayerController.Reload(new PlayerEventArgs(PlayerEventType.Reload, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.E) && !PrevKeyboardState.IsKeyDown(Keys.E)) _player.PlayerController.NextWeapon(new PlayerEventArgs(PlayerEventType.NextWeapon, gameTime, _blueProjectilesSpritesheet));
            if (CurrentKeyboardState.IsKeyDown(Keys.Q) && !PrevKeyboardState.IsKeyDown(Keys.Q)) _player.PlayerController.PreviousWeapon(new PlayerEventArgs(PlayerEventType.PreviousWeapon, gameTime, _blueProjectilesSpritesheet)); 
            if (CurrentMouseState.LeftButton == ButtonState.Pressed) _player.PlayerController.Shoot(new PlayerEventArgs(PlayerEventType.Shoot, gameTime, _blueProjectilesSpritesheet));
        }

        private void UpdateInputMethods()
        {
            PrevKeyboardState = CurrentKeyboardState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
        }
        #endregion
    }
}