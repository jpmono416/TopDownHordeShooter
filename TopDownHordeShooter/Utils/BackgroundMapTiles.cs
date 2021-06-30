using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils
{
    public class BackgroundMapTiles
    {
        // X,Y positions of all instances of the floor sprite
        private readonly List<Vector2> _floorTilesPositions = new List<Vector2>();
        public Texture2D FloorTexture;
        
        public void Initialize(Texture2D texture)
        {
            FloorTexture = texture;
            _floorTilesPositions.Add(new Vector2(0,0));
            _floorTilesPositions.Add(new Vector2(FloorTexture.Width,0));
            _floorTilesPositions.Add(new Vector2(0,FloorTexture.Height));
            _floorTilesPositions.Add(new Vector2(FloorTexture.Width,FloorTexture.Height));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in _floorTilesPositions)
            {
                spriteBatch.Draw(FloorTexture, tile, null, Color.White, 0f, Vector2.Zero,
                    1f,SpriteEffects.None, 0f);
            }
        }
    }
}