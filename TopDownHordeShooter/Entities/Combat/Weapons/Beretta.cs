using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class Beretta : BaseWeapon
    {
        public Beretta()
        {
            // Spritesheet coordinates: 109,7 - 140,20 (31,13 size)
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(0, 24, 12, 10);
            MaxBulletsPerMag = 9;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 5;
        }
    }
}