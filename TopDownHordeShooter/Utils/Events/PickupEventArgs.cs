using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Utils.Events
{
    public enum PickupEventType
    {
        Spawn,
        Collected,
        ApplyEffect,
        
    }
    public class PickupEventArgs : EventArgs
    {
        public PickupEventType EventType;
        public GameTime GameTime;
        public Player Player;

        public PickupEventArgs(PickupEventType eventType, GameTime gameTime, Player player)
        {
            GameTime = gameTime;
            Player = player;
            EventType = eventType;
        }
    }
}