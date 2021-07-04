using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Combat.Projectiles
{
    public class BlueProjectile : BaseProjectile
    {
        public BlueProjectile(Texture2D spritesheet)
        {
            ProjectileSpeed = 15f;
            SpriteSheet = spritesheet;
            ProjectileAnimation = new Animation();
            ProjectileAnimation.Initialize(SpriteSheet, new Vector2(10f, 27f), 40, 30, 5, AnimationTypes.Projectile, scale: 0.6f);
            Damage = 20;
        }
    }
}