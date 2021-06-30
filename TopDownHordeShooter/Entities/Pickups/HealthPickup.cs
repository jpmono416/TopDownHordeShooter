using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class HealthPickup : BasePickup
    {

        public HealthPickup()
        {
            Width = 16;
            SpriteSheetCoordinates = new Rectangle(0, 0, Width, Height);  
            //SpawnTime = TimeSpan.FromMinutes(2);
            SpawnTime = TimeSpan.FromSeconds(20);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            player.Health += 25;
        }
    }
}