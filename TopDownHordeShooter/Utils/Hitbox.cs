using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils
{
    public class Hitbox
    {
        private int _id;
        private int HitboxId { get; }
        public ColliderType ColliderType;

        private Texture2D _texture;
        
        // Transform details used for assignment and simpler calculations
        public Vector2 Position;
        private readonly float _width;
        private readonly float _height;
        
        // Internal fields for accurate collisions calculations
        private float Left => Position.X;
        private float Right => Position.X + _width;
        private float Top => Position.Y;
        private float Bottom => Position.Y + _height;

        private bool _collides;
        public Hitbox(Vector2 pos, float width, float height, ColliderType type)
        {
            Position = pos;
            _width = width;
            _height = height;
            
            GenerateId();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _texture.SetData(_collides ? new[] {Color.Red} : new[] {Color.DarkSlateGray});

            spriteBatch.Draw(_texture, new Rectangle((int) Position.X, (int) Position.Y, (int) _width, (int) _height), Color.White);
        }
        
        private void GenerateId() => _id = HitboxCounter.GetInstance().CurrentCounter;

        /**
         * Set private property collides for internal calculations and return it
         * for handling collisions externally.
         */
        public bool Collides(Hitbox other)
        {
            
            _collides = Right > other.Left
                       && Left < other.Right
                       && Bottom > other.Top 
                       && Top < other.Bottom;

            return _collides;
        }
        public static bool operator ==(Hitbox a, Hitbox b) => a.HitboxId == b.HitboxId;
        public static bool operator !=(Hitbox a, Hitbox b) => !(a == b);
    }
    
    public enum ColliderType
    {
        Enemy,
        Player,
        Projectile,
        EnemyProjectile,
        Pickup,
        PlayerSafeZone,
        None
    }
}