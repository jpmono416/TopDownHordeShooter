using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class HealthPickup : BasePickup
    {

        public HealthPickup(int spawnTimeSeconds)
        {
            Width = 16;
            SpriteSheetCoordinates = new Rectangle(0, 0, Width, Height);  
            SpawnTimeSpan = TimeSpan.FromSeconds(spawnTimeSeconds + 30);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            player.Health += 25;
        }
    }
}