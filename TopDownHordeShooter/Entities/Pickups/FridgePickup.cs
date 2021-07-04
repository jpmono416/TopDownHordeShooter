using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class FridgePickup : BasePickup
    {
        public FridgePickup(int spawnTimeSeconds) 
        {
            Width = 9;
            SpriteSheetCoordinates = new Rectangle(56, 0, Width, Height);
            SpawnTimeSpan = TimeSpan.FromSeconds(spawnTimeSeconds + 80);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            player.BaseDamage += 5;
        }
    }
}