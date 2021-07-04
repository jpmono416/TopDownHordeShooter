using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class Minigun : BaseWeapon
    {
        public Minigun()
        {
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(0, 37, 35, 12);
            MaxBulletsPerMag = 200;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 2;
            RateOfFireOffset = 0.2f;
            LevelToUnlock = 11;
        }
    }
}