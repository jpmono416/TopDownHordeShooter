using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class SurprisePickup : BasePickup
    {
        public SurprisePickup(int spawnTimeSeconds)
        {
            Width = 12;
            SpriteSheetCoordinates = new Rectangle(102, 0, Width, Height);
            SpawnTimeSpan = TimeSpan.FromSeconds(spawnTimeSeconds + 60);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            player.BulletsPerMinute += 5;
        }
    }
}