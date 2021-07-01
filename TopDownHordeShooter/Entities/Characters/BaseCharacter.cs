using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Characters
{
    public abstract class BaseCharacter
    {
        // Animations and textures
        public bool CharacterFacingLeft => !(Direction.X > 0);
        protected Texture2D CharacterTexture;
        public Animation CharacterAnimation;
        public List<Animation> Animations;
        
        // Position relative to the upper left side of the screen
        public Vector2 Position;
        
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
        public float Width => CharacterTexture.Width;
        public float Height => CharacterTexture.Height;
        
        // Collision detection
        public Hitbox Hitbox;
        public bool Colliding;
        public ColliderType CollidingWith;
        
        // Minimum time interval to receive damage
        protected TimeSpan ReceiveDamageTimeSpan;
        protected TimeSpan LastDamageReceived;
            
        public bool CanTakeDamage;

        public void TakeDamage(int damage, ColliderType collider, GameTime gameTime)
        {
            // Ensuring right conditions are met for applying damage
            if (!Colliding || !CanTakeDamage || collider == ColliderType.None) return;
            
            // Apply damage and prepare for next collision
            Health -= damage;
            CollidingWith = ColliderType.None;
            Colliding = false;
            CanTakeDamage = false;
            CharacterAnimation = Animations[1];
            LastDamageReceived = gameTime.TotalGameTime;

            if(Health <= 20) CharacterAnimation = Animations[2];
            
            
            // If dead
            if (Health > 0) return;
            Active = false;
            Hitbox.ColliderType = ColliderType.None;
        }

        protected void DamageCooldown(GameTime gameTime)
        {
            // Allow player to take damage again after cooldown
            if (CanTakeDamage || gameTime.TotalGameTime - LastDamageReceived < ReceiveDamageTimeSpan) return;
            
            LastDamageReceived = gameTime.TotalGameTime;
            CanTakeDamage = true;
            CharacterAnimation = Animations[0];
        }
        public abstract void Initialize(List<Texture2D> animationSpriteSheets, Vector2 position);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}