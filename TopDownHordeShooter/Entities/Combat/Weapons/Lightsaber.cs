using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class LightSaber : BaseWeapon
    {
        
        public LightSaber()
        {
            // Spritesheet coordinates: 65,0 - 96,6 (31,6 size)
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(65, 0, 31, 6);
        }
    }
}