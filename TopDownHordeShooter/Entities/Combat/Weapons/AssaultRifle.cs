using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class AssaultRifle : BaseWeapon
    {
        public AssaultRifle()
        {
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(109, 7, 31, 13);
            MaxBulletsPerMag = 30;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 5;
        } 
    }
}