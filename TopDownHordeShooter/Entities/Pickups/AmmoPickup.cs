using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class AmmoPickup : BasePickup
    {
        public AmmoPickup(int spawnTimeSeconds)
        {
            Width = 23;
            SpriteSheetCoordinates = new Rectangle(32, 0, Width, Height);
            SpawnTimeSpan = TimeSpan.FromSeconds(spawnTimeSeconds + 20);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            // Reload and add an extra magazine
            player.CurrentWeapon.RemainingBullets += player.CurrentWeapon.MaxBulletsPerMag;
            player.CurrentWeapon.CurrentBulletsInMag = player.CurrentWeapon.MaxBulletsPerMag;
        }
    }
}