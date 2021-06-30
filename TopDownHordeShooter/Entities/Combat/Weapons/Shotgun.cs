using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class Shotgun : BaseWeapon
    {
        public Shotgun()
        {
            ActiveProjectiles = new List<BaseProjectile>();
            // Spritesheet coordinates: 109,7 - 140,20 (31,13 size)
            SpriteSheetCoordinates = new Rectangle(0, 10, 27, 13);
            MaxBulletsPerMag = 8;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 4;
        }
    }
}