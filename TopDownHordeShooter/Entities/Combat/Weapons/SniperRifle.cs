using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class SniperRifle : BaseWeapon
    {
        public SniperRifle()
        {
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(30, 7, 48, 14);
            MaxBulletsPerMag = 5;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 6;
        }
    }
}