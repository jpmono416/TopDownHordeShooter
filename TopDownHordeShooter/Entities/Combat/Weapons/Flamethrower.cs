using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public class Flamethrower : BaseWeapon
    {
        public Flamethrower()
        {
            // Spritesheet coordinates: 109,7 - 140,20 (31,13 size)
            ActiveProjectiles = new List<BaseProjectile>();
            SpriteSheetCoordinates = new Rectangle(0, 50, 51, 30);
            MaxBulletsPerMag = 500;
            CurrentBulletsInMag = MaxBulletsPerMag;
            RemainingBullets = MaxBulletsPerMag * 5;
            LevelToUnlock = 5;
            RateOfFireOffset = 0.3f;
            DamageOffset = -10;
            ProjectileDuration = TimeSpan.FromMilliseconds(500);
        }
    }
}