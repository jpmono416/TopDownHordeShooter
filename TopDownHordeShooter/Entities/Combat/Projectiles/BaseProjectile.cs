using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Combat.Projectiles
{
    public class BaseProjectile
    {
        public Texture2D SpriteSheet;
        public Animation ProjectileAnimation;
        public Vector2 Position;
        public Vector2 Direction;
        private const float ProjectileSpeed = 15.0f;

        public int Damage;
        
        public bool Active;
        public Hitbox Hitbox;

        // Time when the projectile is created
        public TimeSpan SpawnTime; 
        protected TimeSpan ProjectileDuration;


        private int Width => ProjectileAnimation.FrameWidth;
        private int Height => ProjectileAnimation.FrameHeight;

        public void Initialize(Vector2 position, Vector2 direction)
        {
            Direction = direction;
            Position = position;
            Active = true;
            Hitbox = new Hitbox(Position, Width, Height, ColliderType.Projectile);
            ProjectileDuration = TimeSpan.FromSeconds(1);
        }
        
        public void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            // Position
            Position += ProjectileSpeed * Direction;
            Hitbox.Position = Position;
            
            // Run animation
            ProjectileAnimation.Position = Position;
            ProjectileAnimation.Update(gameTime);
            
            if (gameTime.TotalGameTime - SpawnTime >= ProjectileDuration) Active = false;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            ProjectileAnimation.Draw(spriteBatch);
        }

    }
}