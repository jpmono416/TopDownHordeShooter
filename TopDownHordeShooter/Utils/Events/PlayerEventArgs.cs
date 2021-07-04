using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils.Events
{
    public enum PlayerEventType
    {
        Reload,
        Shoot,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        NextWeapon,
        PreviousWeapon,
        GainXp
    }
    public class PlayerEventArgs : EventArgs
    {
        public readonly PlayerEventType EventType;
        public readonly GameTime GameTime;
        public readonly Texture2D BulletsSpritesheet;
        public readonly int XpAmount;

        public PlayerEventArgs(PlayerEventType eventType, GameTime gameTime, Texture2D bulletsSpritesheet, int xpAmount = 0)
        {
            EventType = eventType;
            GameTime = gameTime;
            BulletsSpritesheet = bulletsSpritesheet;
            XpAmount = xpAmount;
        } 
    }
}