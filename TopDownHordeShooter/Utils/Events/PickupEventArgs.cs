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
        public readonly PickupEventType EventType;
        public readonly GameTime GameTime;
        public readonly Player Player;

        public PickupEventArgs(PickupEventType eventType, GameTime gameTime, Player player)
        {
            GameTime = gameTime;
            Player = player;
            EventType = eventType;
        }
    }
}