using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Combat.Projectiles
{
    public class BaseProjectile
    {
        protected Texture2D SpriteSheet;
        protected Animation ProjectileAnimation;
        private Vector2 _position;
        private Vector2 _direction;
        protected float ProjectileSpeed;

        public int Damage;
        
        public bool Active;
        public Hitbox Hitbox;

        // Time when the projectile is created
        public TimeSpan SpawnTime;
        private TimeSpan _projectileDuration;


        private int Width => ProjectileAnimation.FrameWidth;
        private int Height => ProjectileAnimation.FrameHeight;

        public void Initialize(Vector2 position, Vector2 direction, TimeSpan duration)
        {
            _direction = direction;
            _position = position;
            Active = true;
            Hitbox = new Hitbox(_position, Width, Height, ColliderType.EnemyProjectile);
            _projectileDuration = duration;
        }
        
        public void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            // Position
            _position += ProjectileSpeed * _direction;
            Hitbox.Position = _position;
            
            // Run animation
            ProjectileAnimation.FacingLeft = _direction.X < 0;
            ProjectileAnimation.Position = _position;
            ProjectileAnimation.Update(gameTime);
            
            if (gameTime.TotalGameTime - SpawnTime >= _projectileDuration) Active = false;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            ProjectileAnimation.Draw(spriteBatch);
        }

    }
}