using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;

namespace TopDownHordeShooter.Entities.Combat.Projectiles
{
    public class BlueProjectile : BaseProjectile
    {
        public BlueProjectile(Texture2D spritesheet)
        {
            SpriteSheet = spritesheet;
            ProjectileAnimation = new Animation();
            ProjectileAnimation.Initialize(SpriteSheet, new Vector2(11f, 5), 50, 80, 4, 30, Color.White, 0.5f, true);
            Damage = 20;
        }
    }
}