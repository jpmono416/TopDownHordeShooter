using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils
{
    public class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D _spriteStrip;
        // The scale used to display the sprite strip
        float _scale;
        // The time since we last updated the frame
        int _elapsedTime;
        // The time we display a frame until the next one
        int _frameTime;
        // The number of frames that the animation contains
        int _frameCount;
        // The index of the current frame we are displaying
        int _currentFrame;
        // The color of the frame we will be displaying
        Color _color;
        // The area of the image strip we want to display
        Rectangle _sourceRect = new Rectangle();
        // The area where we want to display the image strip in the game
        Rectangle _destinationRect = new Rectangle();
        // Width of a given frame
        public int FrameWidth;
        // Height of a given frame
        public int FrameHeight;
        // The state of the Animation
        public bool Active;
        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;
       
        // Width of a given frame
        public Vector2 Position;
        
        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int
            frameCount, int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this._color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this._frameCount = frameCount;
            this._frameTime = frametime;
            this._scale = scale;
            Looping = looping;
            Position = position;
            _spriteStrip = texture;
            // Set the time to zero
            _elapsedTime = 0;
            _currentFrame = 0;
            // Set the Animation to active by default
            Active = true;
        }
        
        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (!Active) return;
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
                    if (!Looping)
                        Active = false;
                }
                // Reset the elapsed time to zero
                _elapsedTime = 0;
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
                _sourceRect = new Rectangle(_currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
                
            _destinationRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(FrameWidth * _scale), (int)(FrameHeight * _scale));
            
            //_destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * _scale) / 2, (int)Position.Y
                    //- (int)(FrameHeight * _scale) / 2, (int)(FrameWidth * _scale), (int)(FrameHeight * _scale));
        }

        
        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (!Active) return;
            spriteBatch.Draw(_spriteStrip, _destinationRect, _sourceRect, _color);
        }
    }
}