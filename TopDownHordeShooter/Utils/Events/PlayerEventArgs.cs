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
        PreviousWeapon
    }
    public class PlayerEventArgs : EventArgs
    {
        public PlayerEventType EventType;
        public GameTime GameTime;
        public Texture2D BulletsSpritesheet;

        public PlayerEventArgs(PlayerEventType eventType, GameTime gameTime, Texture2D bulletsSpritesheet)
        {
            EventType = eventType;
            GameTime = gameTime;
            BulletsSpritesheet = bulletsSpritesheet;
        } 
    }
}