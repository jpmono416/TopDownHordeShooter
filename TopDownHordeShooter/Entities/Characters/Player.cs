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
        public BaseWeapon CurrentWeapon => Weapons[_currentWeaponIndex];
        private int _currentWeaponIndex;
        public int Level;
        public int XpPoints;
        public int NextLevelXpRequired;

        public readonly PlayerController PlayerController;

        public TimeSpan RateOfFire
        {
            get => TimeSpan.FromMilliseconds(1000* 60 / BulletsPerMinute);
            private set => BulletsPerMinute = 1000* 60 / value.Milliseconds;
        }
        public int BulletsPerMinute;

        private TimeSpan _lastBulletShot;
        public List<BaseWeapon> Weapons;

        public Player(PlayerStats stats)
        {
            Level = stats.Level;
            Health = stats.Health;
            MoveSpeed = stats.BaseMoveSpeed;
            BaseDamage = stats.BaseDamage;
            Scale = 1.3f;
            RateOfFire = TimeSpan.FromMilliseconds(1000 * 60 / stats.BaseFireRate);
            ReceiveDamageTimeSpan = TimeSpan.FromSeconds(3);
            LastDamageReceived = TimeSpan.Zero;
            ChangeAnimationTimeSpan = TimeSpan.FromMilliseconds(200);
            XpPoints = 0;
            NextLevelXpRequired = CalculateNextLevelXp();;
            _currentWeaponIndex = 0;
            PlayerController = new PlayerController();
            CharacterAnimation = new Animation();
            Active = true;
        }
        public override void Initialize(List<Texture2D> animationSpritesheets, Vector2 position, int widthHeight)
        {
            var tempIdleAnimation = new Animation();
            var tempTakeDamageAnimation = new Animation();
            var tempWalkingAnimation = new Animation();
            var tempDeathAnimation = new Animation();
            
            tempIdleAnimation.Initialize(animationSpritesheets[0], position, widthHeight, widthHeight, 4, AnimationTypes.Idle, scale: Scale);
            tempTakeDamageAnimation.Initialize(animationSpritesheets[1], position, widthHeight, widthHeight, 2, AnimationTypes.TakingDamage, scale: Scale);
            tempWalkingAnimation.Initialize(animationSpritesheets[2], position, widthHeight, widthHeight, 6, AnimationTypes.Movement, scale: Scale);
            tempDeathAnimation.Initialize(animationSpritesheets[3], position, widthHeight, widthHeight, 6, AnimationTypes.Death, scale: Scale);
            
            Animations = new List<Animation>{tempIdleAnimation, tempTakeDamageAnimation, tempWalkingAnimation, tempDeathAnimation};

            CharacterAnimation = Animations[0];

            Position = position;
            Hitbox = new Hitbox(position, Width, Height, ColliderType.Player);
            NextWeapon();
            CurrentWeapon.Active = true;
            AddController(PlayerController);
            
        }

        public override void Update(GameTime gameTime)
        {
           // Update positions
            Hitbox.Position = Position;
            CurrentWeapon.PlayerPosition = Position;
            CharacterAnimation.Position = Position;
            
            // Check damage timer
            DamageCooldown(gameTime);
            
            // Experience
            UpdateLevels();
            
            // Default animation
            ChangeAnimation(gameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Idle));
            CharacterAnimation.FacingLeft = Direction.X < 0; // For some reason
            CharacterAnimation.Update(gameTime);
            CurrentWeapon.Update(gameTime);
        }
        

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw if player isn't alive
            if(!Active) return;

            CharacterAnimation.Draw(spriteBatch);
            // Draw weapon
            CurrentWeapon.WeaponFacingLeft = CharacterFacingLeft;
            CurrentWeapon.Draw(spriteBatch);
        }

        #region EXP

        private void GainXp(int amount) => XpPoints += amount;

        private void UpdateLevels()
        {
            if (XpPoints < NextLevelXpRequired) return;
            ++Level;
            
            // Take away the level's required EXP and keep any remaining points
            XpPoints -= NextLevelXpRequired;
            NextLevelXpRequired = CalculateNextLevelXp();
            
            ApplyLevelUpgrades();

        }

        private void ApplyLevelUpgrades()
        {
            Health += Level * 2;
            MoveSpeed += 0.05f;
            BulletsPerMinute += 3;
            BaseDamage += 2;
            
            
        }
        
        private int CalculateNextLevelXp()
        {
            const int minXp = 100;
            return minXp + Level * 10;
        }
        #endregion
        

        #region Input

        private void AddController(PlayerController playerController) =>
            playerController.Changed += OnChanged;

        private void OnChanged(object sender, PlayerEventArgs args)
        {
            switch (args.EventType)
            {
                case PlayerEventType.MoveLeft:
                    if (!Active) return;
                    
                    Direction = new Vector2(-1f, 0f);
                    ChangeAnimation(args.GameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Movement), true);
                    Position.X -= MoveSpeed;
                    break;
                
                case PlayerEventType.MoveRight:
                    if (!Active) return;
                    
                    Direction = new Vector2(1f, 0f);
                    ChangeAnimation(args.GameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Movement), true);
                    Position.X += MoveSpeed;
                    break;
                
                case PlayerEventType.MoveUp:
                    if (!Active) return;
                    ChangeAnimation(args.GameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Movement), true);
                    Position.Y -= MoveSpeed;
                    break;
                
                case PlayerEventType.MoveDown:
                    if (!Active) return;
                    ChangeAnimation(args.GameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Movement), true);
                    Position.Y += MoveSpeed;
                    break;
                case PlayerEventType.Reload:
                    CurrentWeapon.Reload(args.GameTime);
                    break;
                case PlayerEventType.Shoot:
                    if (args.GameTime.TotalGameTime - _lastBulletShot < RateOfFire * CurrentWeapon.RateOfFireOffset) return;
                    _lastBulletShot = args.GameTime.TotalGameTime;
                    CurrentWeapon.Shoot(args.GameTime, CharacterFacingLeft, args.BulletsSpritesheet);
                    break;
                case PlayerEventType.NextWeapon:
                    NextWeapon();
                    break;
                case PlayerEventType.PreviousWeapon:
                    PreviousWeapon();
                    break;
                case PlayerEventType.GainXp:
                    GainXp(args.XpAmount);
                    break;
            }

        }
        private void NextWeapon()
        {
            var nextIndex = Weapons.Count <= _currentWeaponIndex + 1 ? 0 : _currentWeaponIndex + 1;
            
            // Make sure it's updated
            Weapons[nextIndex].UpdateUnlocked(Level);
            var isNextUnlocked = Weapons[nextIndex].Unlocked;
            
            while (!isNextUnlocked)
            {
                _currentWeaponIndex = nextIndex;
                NextWeapon();
                return;
            }

            // Reached an unlocked weapon
            CurrentWeapon.Active = false;
            _currentWeaponIndex = nextIndex;
            CurrentWeapon.Active = true;
        }
        
        private void PreviousWeapon()
        {
            var nextIndex = _currentWeaponIndex - 1 < 0 ? Weapons.Count -1 : _currentWeaponIndex - 1;
            
            // Make sure it's updated
            Weapons[nextIndex].UpdateUnlocked(Level);
            var isNextUnlocked = Weapons[nextIndex].Unlocked;
            
            while (!isNextUnlocked)
            {
                _currentWeaponIndex = nextIndex;
                PreviousWeapon();
                return;
            }

            // Reached an unlocked weapon
            CurrentWeapon.Active = false;
            _currentWeaponIndex = nextIndex;
            CurrentWeapon.Active = true;
        }
        
        #endregion
    }
}