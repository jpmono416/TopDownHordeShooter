using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Entities.Pickups
{
    public class SurprisePickup : BasePickup
    {
        public SurprisePickup()
        {
            Width = 12;
            SpriteSheetCoordinates = new Rectangle(102, 0, Width, Height);
            
            //SpawnTime = TimeSpan.FromMinutes(2);
            SpawnTime = TimeSpan.FromSeconds(30);
        }

        protected override void ApplyEffect(Player player, GameTime gameTime)
        {
            Active = false;
            LastGone = gameTime.TotalGameTime;

            player.BulletsPerMinute += 5;
        }
    }
}