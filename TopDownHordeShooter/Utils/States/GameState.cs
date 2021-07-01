using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownHordeShooter.Entities.Characters;
using TopDownHordeShooter.Entities.Combat.Weapons;
using TopDownHordeShooter.Entities.Pickups;
using TopDownHordeShooter.Utils.Events;
using TopDownHordeShooter.Utils.UI;

namespace TopDownHordeShooter.Utils.States
{
    public class GameState : BaseState
    {
        private ContentManager Content;

        private SpriteBatch _spriteBatch;
        
        // Background
        Texture2D _backgroundTexture;
        private readonly BackgroundMapTiles _background;
        
        // Enemies
        List<Texture2D> _enemySpriteSheets;
        private readonly List<Enemy> _enemies;

        // Player
        private Player _player;
        private Texture2D _playerTexture;
        private Texture2D _playerLeftTexture;
        
        // Weapons
        private Texture2D _weaponsSpritesheet;
        private int _currentWeaponIndex;
        private bool _weaponChanged;
        
        // Projectiles
        private Texture2D _projectilesSpritesheet;

        // Pickups
        private Texture2D _pickupsSpritesheet;
        private List<BasePickup> _pickups;

        private TimeSpan _changeWeaponTimespan;
        private TimeSpan _previousWeaponChangeTime;

        private GraphicsDevice GraphicsDevice;
        
        // Input
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }
        
        // UI text
        private Text uiText;
        
        // Game data
        private GameData _gameData;

        // Enemy waves manager
        public GameLoop GameLoop;

        private bool GamePaused;
        
        public GameState(HordeShooterGame game, GraphicsDevice graphicsDevice, ContentManager content) 
            : base(game, graphicsDevice, content)
        {
            GraphicsDevice = graphicsDevice;
            Content = content;
            _spriteBatch = game.SpriteBatch;
            _gameData = InitializeGameData();

            _background = new BackgroundMapTiles();
            
            _player = new Player(_gameData.PlayerStats);
            _enemies = new List<Enemy>();
            _enemySpriteSheets = new List<Texture2D>();
            
            LoadContent();

            // Input
            _weaponChanged = false;

            // Timespans
            _changeWeaponTimespan = TimeSpan.FromSeconds(2);
            _previousWeaponChangeTime = TimeSpan.Zero;

            Content.RootDirectory = "Content";
            GameLoop = new GameLoop();
            
        }

        private void LoadContent()
        {
            _backgroundTexture = Content.Load<Texture2D>("Graphics\\stone_floor");
            
            // Text
            uiText = new Text (Content);
            
            // Player
            var playerSpawnPosition = Vector2.Zero;
                //new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            
            
            // Player textures and weapons inventory
            _playerTexture = Content.Load<Texture2D>("Graphics\\Character-R");
            _weaponsSpritesheet = Content.Load<Texture2D>("Graphics\\Weapons");
            _player.Weapons = BaseWeapon.InitAllWeapons(_weaponsSpritesheet);
            _player.Initialize(new List<Texture2D>{_playerTexture}, playerSpawnPosition);
            
            // Projectiles
            _projectilesSpritesheet = Content.Load<Texture2D>("Graphics\\Shots");
            
            // Pickups
            _pickupsSpritesheet = Content.Load<Texture2D>("Graphics\\Pickups");
            _pickups = BasePickup.InitAllPickups(_pickupsSpritesheet);

            // Walker enemy texture
            _enemySpriteSheets.Add(Content.Load<Texture2D>("Graphics\\Zombie-Walking-Animation"));
            _enemySpriteSheets.Add(Content.Load<Texture2D>("Graphics\\Zombie-Damage-Animation"));
            _enemySpriteSheets.Add(Content.Load<Texture2D>("Graphics\\Zombie-Dying-Animation"));
            

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

            if (GamePaused) return;
            // Check if weapon change timer is up
            SetChangeWeaponAllowed(gameTime);

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
                _player.GainXP(toBeRemoved.ScoreGiven);
                _enemies.Remove(toBeRemoved);
            }
            
            
            // Pickups
            foreach (var pickup in _pickups)
                pickup.Update(gameTime);

            CalculateCollisions(gameTime);
            
            // Update game loop
            if (_enemies.Count == 0) GameLoop.WaveCompleted = true;
            GameLoop.Update(gameTime);
            if (GameLoop.TimerOver)
            {
                GameLoop.TimerOver = false;
                SpawnEnemies();
            }
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (GamePaused) return;
            
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background
            _background.Draw(spriteBatch);
            
            // Player
            _player.Draw(spriteBatch);

            // Enemies
            foreach (var enemy in _enemies) enemy.Draw(spriteBatch);

            // Pickups
            foreach (var pickup in _pickups) pickup.Draw(spriteBatch);
            
            // UI
            DrawUI(spriteBatch);
            
            spriteBatch.End();
        }
        
        private static GameData InitializeGameData()
        {
            var enemyDataString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\EnemyData.json");
            var playerStatsString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\PlayerStats.json");
            var difficultyDataString = FileLoader.ReadTextFileComplete(@"..\..\..\Data\DifficultyData.json");
            
            var enemyDataObj = (EnemyData) FileLoader.JSONToObject(enemyDataString, typeof(EnemyData));
            var playerStatsSObj = (PlayerStats) FileLoader.JSONToObject(playerStatsString, typeof(PlayerStats));
            var difficultyDataObj = (DifficultyData) FileLoader.JSONToObject(difficultyDataString, typeof(DifficultyData));

            return new GameData(playerStatsSObj, enemyDataObj, difficultyDataObj);
        }
        
        private void DrawUI(SpriteBatch spriteBatch)
        {
            uiText.DrawString("Level: " + _player.Level,new Vector2(GraphicsDevice.Viewport.Width-150,20),Color.White,2,false, spriteBatch);
            uiText.DrawString("Exp: " + _player.XPPoints + " / " + _player.NextLevelXPRequired,new Vector2(GraphicsDevice.Viewport.Width-200,50),Color.White,2,false, spriteBatch);
            uiText.DrawString("Health: " + _player.Health,new Vector2(20,20),Color.White,2,false, spriteBatch);
            uiText.DrawString("Rate of fire: " + ( 1000 * 60 / _player.RateOfFire.Milliseconds) + " bpm",new Vector2(20,50),Color.White,2,false, spriteBatch);
            uiText.DrawString("Damage: " + _player.BaseDamage,new Vector2(20,80),Color.White,2,false, spriteBatch);
            uiText.DrawString("Ammo: " + _player.CurrentWeapon.CurrentBulletsInMag + " / " + _player.CurrentWeapon.RemainingBullets,new Vector2(20,110),Color.White,2,false, spriteBatch);
            uiText.DrawString("Current wave: " + GameLoop.CurrentWave,new Vector2(20,GraphicsDevice.Viewport.Height - 40),Color.Crimson,2,false, spriteBatch);

            // Display health counters
            foreach (var enemy in _enemies)
            {
                uiText.DrawString(enemy.Health.ToString(),new Vector2(enemy.Position.X + enemy.CharacterAnimation.FrameWidth / 2, enemy.Position.Y - enemy.CharacterAnimation.FrameHeight / 2),Color.Firebrick,1,false, spriteBatch);
                uiText.DrawString(enemy.Position.ToString(),new Vector2(enemy.Position.X + enemy.CharacterAnimation.FrameWidth / 2, enemy.Position.Y + enemy.CharacterAnimation.FrameHeight / 2),Color.Green,1,false, spriteBatch);
            }
        }
        
        #region Game Actions
        private void SpawnEnemies()
        {
            // Spawn walker enemies
            for (var i = 0; i <= 5; i++)
            {
                var r = new Random();
                
                var enemyPosition = new Vector2(r.Next(0, GraphicsDevice.Viewport.Width),
                    r.Next(0, GraphicsDevice.Viewport.Height)); 
                _enemies.Add(new WalkerEnemy(_gameData.EnemyData));
                
                _enemies[^1].Initialize(_enemySpriteSheets, enemyPosition);
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
                }

                // Calculate projectile collisions
                foreach (var projectile in _player.CurrentWeapon.ActiveProjectiles.Where(projectile => projectile.Hitbox.Collides(enemy.Hitbox)))
                {
                    // Inflict damage to enemy
                    enemy.Colliding = true;
                    enemy.CollidingWith = ColliderType.Projectile;
                    enemy.TakeDamage(projectile.Damage, projectile.Hitbox.ColliderType, gameTime);
                    
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
        private void SetChangeWeaponAllowed(GameTime gameTime)
        {
            if (!_weaponChanged) return;
            
            _previousWeaponChangeTime = gameTime.TotalGameTime;
            _weaponChanged = false;
        }
        #endregion
        #region Input
        private void CheckInput(GameTime gameTime)
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape)) GamePaused = !GamePaused;
            if (CurrentKeyboardState.IsKeyDown(Keys.A)) _player.PlayerController.MoveLeft(new PlayerEventArgs(PlayerEventType.MoveLeft, gameTime, _projectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.W)) _player.PlayerController.MoveUp(new PlayerEventArgs(PlayerEventType.MoveUp, gameTime, _projectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.S)) _player.PlayerController.MoveDown(new PlayerEventArgs(PlayerEventType.MoveDown, gameTime, _projectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.D)) _player.PlayerController.MoveRight(new PlayerEventArgs(PlayerEventType.MoveRight, gameTime, _projectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.R)) _player.PlayerController.Reload(new PlayerEventArgs(PlayerEventType.Reload, gameTime, _projectilesSpritesheet)); 
            if (CurrentKeyboardState.IsKeyDown(Keys.E)) _player.PlayerController.NextWeapon(new PlayerEventArgs(PlayerEventType.NextWeapon, gameTime, _projectilesSpritesheet));
            if (CurrentKeyboardState.IsKeyDown(Keys.Q)) _player.PlayerController.PreviousWeapon(new PlayerEventArgs(PlayerEventType.PreviousWeapon, gameTime, _projectilesSpritesheet)); 
            if (CurrentMouseState.LeftButton == ButtonState.Pressed) _player.PlayerController.Shoot(new PlayerEventArgs(PlayerEventType.Shoot, gameTime, _projectilesSpritesheet));
        }

        private void UpdateInputMethods()
        {
            PrevKeyboardState = CurrentKeyboardState;
            PrevMouseState = CurrentMouseState;
            
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
        }
        #endregion
    }
}