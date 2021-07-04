using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils
{
    public class Animation
    {
        // The image representing the collection of images used for animation
        private Texture2D _spriteStrip;
        // The scale used to display the sprite strip
        private float _scale;
        // The time since we last updated the frame
        private int _elapsedTime;
        // The time we display a frame until the next one
        private int _frameTime;
        // The number of frames that the animation contains
        private int _frameCount;
        // The index of the current frame we are displaying
        private int _currentFrame;
        // The color of the frame we will be displaying
        private Color _color;
        // The area of the image strip we want to display
        private Rectangle _sourceRect;
        // The area where we want to display the image strip in the game
        private Rectangle _destinationRect;
        // Width of a given frame
        public int FrameWidth;
        // Height of a given frame
        public int FrameHeight;
        // The state of the Animation
        private bool _active;
        // Determines if the animation will keep playing or deactivate after one run
        private bool _looping;
        public bool FacingLeft;
        
        public AnimationTypes AnimationType;
        // Width of a given frame
        public Vector2 Position;
        
        public void Initialize(Texture2D spriteSheet, Vector2 position, int frameWidth, int frameHeight, int
            frameCount, AnimationTypes animationType, bool looping = true, float scale = 1f, int frameTime = 30)
        {
            // Keep a local copy of the values passed in
            _color = Color.White;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _scale = scale;
            _looping = looping;
            Position = position;
            _spriteStrip = spriteSheet;
            AnimationType = animationType;
            
            // Set the time to zero
            _elapsedTime = 0;
            _currentFrame = 0;
            
            // Set the Animation to active by default
            _active = true;
        }
        
        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (!_active) return;
            // Update the elapsed time
            _elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (_elapsedTime > _frameTime)
            {
                // Move to the next frame
                _currentFrame++;
                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (_currentFrame == _frameCount)
                {
                    _currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (!_looping)
                        _active = false;
                }
                // Reset the elapsed time to zero
                _elapsedTime = 0;
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
                _sourceRect = new Rectangle(_currentFrame * FrameWidth, 0
                    , FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
                
            _destinationRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(FrameWidth * _scale), (int)(FrameHeight * _scale));
        }

        
        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (!_active) return;
            spriteBatch.Draw(_spriteStrip, _destinationRect, _sourceRect, _color, 0f,
                Vector2.Zero,  FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }
    }

    public enum AnimationTypes
    {
        Idle,
        Movement,
        TakingDamage,
        Death,
        Projectile,
        NoAnimation
    }
}