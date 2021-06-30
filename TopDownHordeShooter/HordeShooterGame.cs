using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownHordeShooter.Entities.Characters;
using TopDownHordeShooter.Entities.Combat.Projectiles;
using TopDownHordeShooter.Entities.Combat.Weapons;
using TopDownHordeShooter.Entities.Pickups;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.Events;
using TopDownHordeShooter.Utils.Input;
using TopDownHordeShooter.Utils.States;
using TopDownHordeShooter.Utils.UI;

namespace TopDownHordeShooter
{
    public class HordeShooterGame : Game
    {
        #region Properties and fields
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Game states
        private BaseState _currentState;
        private BaseState _nextState;

        public void ChangeState(BaseState state)
        {
            _nextState = state;
        }
        
        // Background
        Texture2D _backgroundTexture;
        private readonly BackgroundMapTiles _background;
        
        // Enemies
        Texture2D _enemyTexture;
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
        
        // Screen dimensions
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;
        
        // Input
        private readonly CommandManager _commandManager;
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

        #endregion

        #region Constructor
        public HordeShooterGame()
        {
            // Load data
            _gameData = InitializeGameData();
            
            //Graphics
            _graphics = new GraphicsDeviceManager(this);
            _background = new BackgroundMapTiles();

            // Entities
            _enemies = new List<Enemy>();
            _player = new Player(_gameData.PlayerStats);

            // Input
            _commandManager = new CommandManager();
            _weaponChanged = false;

            // Timespans
            _changeWeaponTimespan = TimeSpan.FromSeconds(2);
            //_enemySpawnTime = TimeSpan.FromSeconds(5);
            _previousWeaponChangeTime = TimeSpan.Zero;
            //_previousSpawnTime = TimeSpan.Zero;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            GameLoop = new GameLoop();
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("Graphics\\stone_floor");
            
            // Game state
            _currentState = new MenuState(this, _graphics.GraphicsDevice, Content);
            
            
            // Text
            uiText = new Text (Content, new  SpriteBatch (GraphicsDevice));
            
            // Player
            var playerSpawnPosition =
                new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            
            _playerTexture = Content.Load<Texture2D>("Graphics\\Character-R");
            _player.Initialize(_playerTexture, playerSpawnPosition);

            // Init weapon inventory
            _weaponsSpritesheet = Content.Load<Texture2D>("Graphics\\Weapons");
            _player.Weapons = BaseWeapon.InitAllWeapons(_weaponsSpritesheet);

            // Projectiles
            _projectilesSpritesheet = Content.Load<Texture2D>("Graphics\\Shots");
            
            // Pickups
            _pickupsSpritesheet = Content.Load<Texture2D>("Graphics\\Pickups");
            _pickups = BasePickup.InitAllPickups(_pickupsSpritesheet);

            // Walker enemy texture
            var texture1 = Content.Load<Texture2D>("Graphics\\Zombie-Walking-Animation");
            var texture2 = Content.Load<Texture2D>("Graphics\\Zombie-Damage-Animation");
            var texture3 = Content.Load<Texture2D>("Graphics\\Zombie-Dying-Animation");
            
            // Enemies
            SpawnEnemies();

            _background.Initialize(_backgroundTexture); 
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
        #endregion
        
        #region Game Actions
        private void SpawnEnemies()
        {
            // Spawn walker enemies
            for (var i = 0; i <= 15; i++)
            {
                var r = new Random();
                
                var enemyPosition = new Vector2(r.Next(0, GraphicsDevice.Viewport.Width),
                    r.Next(0, GraphicsDevice.Viewport.Height)); 
                _enemies.Add(new WalkerEnemy(_gameData.EnemyData));
                
                _enemies[^1].Initialize(_enemyTexture, enemyPosition);
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
                    _player.TakeDamage(enemy.BaseDamage, enemy.Hitbox.ColliderType);
                }

                // Calculate projectile collisions
                foreach (var projectile in _player.CurrentWeapon.ActiveProjectiles.Where(projectile => projectile.Hitbox.Collides(enemy.Hitbox)))
                {
                    // Inflict damage to enemy
                    enemy.Colliding = true;
                    enemy.CollidingWith = ColliderType.Projectile;
                    enemy.TakeDamage(projectile.Damage, projectile.Hitbox.ColliderType);
                    
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
            
            base.Update(gameTime);
        }
        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            // Game state
            _currentState.Draw(gameTime, _spriteBatch);
            
            if (GamePaused) return;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            // Background
            _background.Draw(_spriteBatch);
            
            // Player
            _player.Draw(_spriteBatch);

            // Enemies
            foreach (var enemy in _enemies) enemy.Draw(_spriteBatch);

            // Pickups
            foreach (var pickup in _pickups) pickup.Draw(_spriteBatch);
            
            // UI
            DrawUI();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawUI()
        {
            uiText.DrawString("Level: " + _player.Level,new Vector2(GraphicsDevice.Viewport.Width-150,20),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Exp: " + _player.XPPoints + " / " + _player.NextLevelXPRequired,new Vector2(GraphicsDevice.Viewport.Width-200,50),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Health: " + _player.Health,new Vector2(20,20),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Rate of fire: " + ( 1000 * 60 / _player.RateOfFire.Milliseconds) + " bpm",new Vector2(20,50),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Damage: " + _player.BaseDamage,new Vector2(20,80),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Ammo: " + _player.CurrentWeapon.CurrentBulletsInMag + " / " + _player.CurrentWeapon.RemainingBullets,new Vector2(20,110),Color.White,2,false, _spriteBatch);
            uiText.DrawString("Current wave: " + GameLoop.CurrentWave,new Vector2(20,GraphicsDevice.Viewport.Height - 40),Color.Crimson,2,false, _spriteBatch);

            // Display health counters
            foreach (var enemy in _enemies)
                uiText.DrawString(enemy.Health.ToString(),new Vector2(enemy.Position.X + enemy.Width / 2, enemy.Position.Y - enemy.Height / 2),Color.Firebrick,1,false, _spriteBatch);
        }
        #endregion
    }
    
    #region Main
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new HordeShooterGame())
                game.Run();
        }
    }
    #endregion
}
