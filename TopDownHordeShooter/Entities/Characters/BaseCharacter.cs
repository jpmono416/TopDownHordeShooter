using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Characters
{
    public abstract class BaseCharacter
    {
        private TimeSpan _totalGameTime;

        // Animations and textures
        protected bool CharacterFacingLeft => !(Direction.X > 0);
        public Animation CharacterAnimation;
        public List<Animation> Animations;


        // Position relative to the upper left side of the screen
        public Vector2 Position;
        public float Scale;


        // Whether object is active 
        public bool Active;


        // Basic stats
        public int Health;
        public float MoveSpeed;


        // The amount of damage inflicted by the character
        public int BaseDamage;

        public Vector2 Direction { get; set; }


        // This is a player stat which is made common as it might be used for enemies 
        public float SpeedMultiplier;


        // Texture

        // Width and height of the sprite
        protected float Width => CharacterAnimation.FrameWidth;
        protected float Height =>CharacterAnimation.FrameHeight;


        // Collision detection
        public Hitbox Hitbox;
        public bool Colliding;
        public ColliderType CollidingWith;


        // Minimum time interval to receive damage
        protected TimeSpan ReceiveDamageTimeSpan;
        protected TimeSpan LastDamageReceived;


        // Minimum time interval to change animations
        protected TimeSpan ChangeAnimationTimeSpan;
        protected TimeSpan LastAnimationChangedTime;

        protected bool CanTakeDamage;
        protected bool CanChangeAnimation => _totalGameTime - LastAnimationChangedTime >= ChangeAnimationTimeSpan;

        public void TakeDamage(int damage, ColliderType collider, GameTime gameTime)
        {
            // Ensuring right conditions are met for applying damage
            if (!Colliding || !CanTakeDamage || collider == ColliderType.None) return;
            
            // Apply damage and prepare for next collision
            Health -= damage;
            CollidingWith = ColliderType.None;
            Colliding = false;
            CanTakeDamage = false;
            ChangeAnimation(gameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.TakingDamage), true);
            LastDamageReceived = gameTime.TotalGameTime;


            // Return if still alive
            if (Health > 0) return;
            
            // Deactivate character
            Active = false;
            Hitbox.ColliderType = ColliderType.None;
            
            // Switch to death animation
            ChangeAnimation(gameTime, Animations.Find(animation => animation.AnimationType == AnimationTypes.Death), true);
        }

        protected void DamageCooldown(GameTime gameTime)
        {
            // Allow character to take damage again after cooldown
            if (CanTakeDamage || gameTime.TotalGameTime - LastDamageReceived < ReceiveDamageTimeSpan) return;
            
            LastDamageReceived = gameTime.TotalGameTime;
            CanTakeDamage = true;
        }

        public void ChangeAnimation(GameTime gameTime, Animation animation, bool forceChange = false)
        {
            // Update timer
            _totalGameTime = gameTime.TotalGameTime;
            
            if (!CanChangeAnimation && !forceChange) return;
            
            LastAnimationChangedTime = gameTime.TotalGameTime;
            CharacterAnimation = animation;
        }
        
        public abstract void Initialize(List<Texture2D> animationSpriteSheets, Vector2 position, int widthHeight);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}