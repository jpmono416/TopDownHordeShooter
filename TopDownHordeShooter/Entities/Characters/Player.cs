using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Combat.Weapons;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.Events;
using TopDownHordeShooter.Utils.GameState;

namespace TopDownHordeShooter.Entities.Characters
{
    public class Player : BaseCharacter
    {
        public BaseWeapon CurrentWeapon => Weapons[_currentWeaponIndex] ?? null;
        private int _currentWeaponIndex;
        public int Level;
        public int XPPoints;
        public int NextLevelXPRequired;

        public PlayerController PlayerController;

        public TimeSpan RateOfFire
        {
            get => TimeSpan.FromMilliseconds(1000* 60 / BulletsPerMinute);
            private set => BulletsPerMinute = 1000* 60 / value.Milliseconds;
        }
        public int BulletsPerMinute;

        public TimeSpan LastBulletShot;
        public List<BaseWeapon> Weapons;

        public Player(PlayerStats stats)
        {
            Level = stats.Level;
            Health = stats.Health;
            MoveSpeed = stats.BaseMoveSpeed;
            BaseDamage = stats.BaseDamage;
            RateOfFire = TimeSpan.FromMilliseconds(1000 * 60 / stats.BaseFireRate);
            ReceiveDamageTimeSpan = TimeSpan.FromSeconds(3);
            LastDamageReceived = TimeSpan.Zero;
            XPPoints = 0;
            NextLevelXPRequired = 0;
            PlayerController = new PlayerController();
            _currentWeaponIndex = 0;
            CharacterAnimation = new Animation();
            Animations = new List<Animation>();
            Active = true;
        }
        public override void Initialize(List<Texture2D> animationSpritesheets, Vector2 position)
        {
            CharacterTexture = animationSpritesheets[0];
            CharacterAnimation.Initialize(CharacterTexture,Vector2.Zero, 64,60,1,30,Color.White,1,true);
            
            Animations.Add(CharacterAnimation);Animations.Add(CharacterAnimation);Animations.Add(CharacterAnimation);
            
            Position = position;
            Hitbox = new Hitbox(position, Width, Height, ColliderType.Player);
            CurrentWeapon.Active = true;
            AddController(PlayerController);
        }

        public override void Update(GameTime gameTime)
        {
           // Update positions
            Hitbox.Position = Position;
            CurrentWeapon.PlayerPosition = Position;
            
            // Check damage timer
            DamageCooldown(gameTime);
            
            // Experience
            UpdateLevels();
            
            CurrentWeapon.Update(gameTime);
        }
        

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw if player isn't alive
            if(!Active) return;

            spriteBatch.Draw(CharacterTexture, Position, null,
                Color.White, 0f, Vector2.Zero, 1f, 
                !CharacterFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            
            // Draw weapon
            CurrentWeapon.WeaponFacingLeft = CharacterFacingLeft;
            CurrentWeapon.Draw(spriteBatch);
        }

        #region EXP
        public void GainXP(int amount) => XPPoints += amount;

        private void UpdateLevels()
        {
            if (XPPoints < NextLevelXPRequired) return;
            ++Level;
            
            // Take away the level's required EXP and keep any remaining points
            XPPoints -= NextLevelXPRequired;
            NextLevelXPRequired = CalculateNextLevelXP();
            
            ApplyLevelUpgrades();

        }

        private void ApplyLevelUpgrades()
        {
            Health += Level * 2;
            MoveSpeed += 0.05f;
            BulletsPerMinute += 3;
            BaseDamage += 2;
            
            
        }
        
        private int CalculateNextLevelXP()
        {
            const int MinXP = 100;
            return MinXP + Level * 10;
        }
        #endregion
        

        #region Movement actions

        public void AddController(PlayerController playerController) =>
            playerController.Changed += OnChanged;

        public void OnChanged(object sender, PlayerEventArgs args)
        {
            switch (args.EventType)
            {
                case PlayerEventType.MoveLeft:
                    if (!Active) return;
                    
                    Direction = new Vector2(-1f, 0f);
                    Position.X -= MoveSpeed;
                    
                    break;
                
                case PlayerEventType.MoveRight:
                    if (!Active) return;
                    
                    Direction = new Vector2(1f, 0f);
                    Position.X += MoveSpeed;
                    
                    break;
                
                case PlayerEventType.MoveUp:
                    if (!Active) return;
                    
                    Position.Y -= MoveSpeed;
                    
                    break;
                
                case PlayerEventType.MoveDown:
                    if (!Active) return;
                    
                    Position.Y += MoveSpeed;
                    
                    break;
                
                case PlayerEventType.Reload:
                    CurrentWeapon.Reload(args.GameTime);
                    break;
                case PlayerEventType.Shoot:
                    if (args.GameTime.TotalGameTime - LastBulletShot < RateOfFire) return;
                    LastBulletShot = args.GameTime.TotalGameTime;
                    CurrentWeapon.Shoot(args.GameTime, CharacterFacingLeft, args.BulletsSpritesheet);
                    break;
                case PlayerEventType.NextWeapon:
                    break;
                case PlayerEventType.PreviousWeapon:
                    
                    break;
            }
        }
        #endregion
        /*
         * Old implementation
         *
        public void MoveRight(EButtonState buttonState, Vector2 amount)
        {
            if (buttonState != EButtonState.PRESSED 
                || Colliding && Hitbox.CollisionSide == CollisionSides.Right
                || !Active) 
                return;

            Direction = new Vector2(1f, 0f);
            Position.X += MoveSpeed;
        }
        
        public void MoveLeft(EButtonState buttonState, Vector2 amount)
        {
            if (buttonState != EButtonState.PRESSED 
                || Colliding && Hitbox.CollisionSide == CollisionSides.Left
                || !Active) 
                return;
            
            Direction = new Vector2(-1f, 0f);
            Position.X -= MoveSpeed;
        }
        
        public void MoveUp(EButtonState buttonState, Vector2 amount)
        {
            if (buttonState != EButtonState.PRESSED 
                || Colliding && Hitbox.CollisionSide == CollisionSides.Top
                || !Active)
                return;
            
            Position.Y -= MoveSpeed;
        }
        
        public void MoveDown(EButtonState buttonState, Vector2 amount)
        {
            if (buttonState != EButtonState.PRESSED 
                || Colliding && Hitbox.CollisionSide == CollisionSides.Bottom
                || !Active)
                return;
            
            Position.Y += MoveSpeed;
        }
        */
    }
}