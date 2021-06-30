using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class Rpg : BaseWeapon
    {
        public Rpg()
        {
            ActiveProjectiles = new List<BaseProjectile>();
            // Spritesheet coordinates: 109,7 - 140,20 (31,13 size)
            SpriteSheetCoordinates = new Rectangle(48, 35, 56, 14);
        }
    }
}