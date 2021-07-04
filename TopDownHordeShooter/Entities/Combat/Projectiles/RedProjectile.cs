using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Combat.Projectiles
{
    public class RedProjectile : BaseProjectile
    {
        public RedProjectile(Texture2D spritesheet)
        {
            
            SpriteSheet = spritesheet;
            ProjectileAnimation = new Animation();
            ProjectileAnimation.Initialize(SpriteSheet, new Vector2(10f, 0), 40, 30, 5, AnimationTypes.Projectile, scale: 1f);
            ProjectileSpeed = 9f;
            Damage = 35;
            
        }
    }
}