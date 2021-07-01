using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils.UI
{
    public class Text
    {
        private readonly SpriteFont _font;

        public Text(ContentManager content)
        {
            _font = content.Load<SpriteFont> ("Fonts\\Arial");
        }
        
        public void DrawString(string text,Vector2 pos, Color color, float scale,bool center, SpriteBatch spriteBatch)
        {
            if (center)
            {
                var stringSize = _font.MeasureString(text) * 0.5f;
                spriteBatch.DrawString(_font, text, pos - (stringSize * scale), color, 0.0f, Vector2.Zero, scale,SpriteEffects.None,0.0f);
            }
            else
            {
                spriteBatch.DrawString(_font, text, pos, color, 0.0f, Vector2.Zero, scale,SpriteEffects.None,0.0f);
            }
        }
    }
}