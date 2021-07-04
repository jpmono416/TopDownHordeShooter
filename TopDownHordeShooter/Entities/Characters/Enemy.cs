using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.EnemyAI;
using TopDownHordeShooter.Utils.GameState;

namespace TopDownHordeShooter.Entities.Characters
{
    public abstract class Enemy : BaseCharacter
    {
        
        // The amount of score the enemy will give to the player
        public readonly int ScoreGiven;

        // FSM for actions
        protected Fsm Fsm;
        
        // Sensor for AI
        private readonly float _visionRange;

        // Store player position for quicker and simpler AI calculations. Should be OK on a PvE game
        public Vector2 PlayerPosition { get; set; }
        protected bool PlayerInRange;

        public int SpawnRate;
        public int MinimumWave;

        // Enemy direction facing towards player, calculated on the spot but needs to be normalized before use

        protected Enemy(EnemyData data, DifficultyData difficulty)
        {
            Health = data.Health;
            BaseDamage = (int) (data.Damage * difficulty.EnemyDamageMultiplier);
            MoveSpeed = data.MoveSpeed;
            _visionRange = data.VisionRange;
            ScoreGiven = data.ScoreGiven;
            SpawnRate = data.SpawnRate;
            MinimumWave = data.MinimumWave;
            PlayerInRange = false;
            CanTakeDamage = true;
            CharacterAnimation = new Animation();
            
            ReceiveDamageTimeSpan = TimeSpan.Zero;
            ChangeAnimationTimeSpan = TimeSpan.FromSeconds(1);
            LastAnimationChangedTime = TimeSpan.Zero;
            
            LastDamageReceived = TimeSpan.Zero;
            
            Active = true;
        }
        
        public override void Initialize(List<Texture2D> animationSpriteSheets, Vector2 position, int widthHeight)
        {
            Position = position;
            var tempIdleAnimation = new Animation();
            var tempWalkAnimation = new Animation();
            var tempHurtAnimation = new Animation();
            var tempDeathAnimation = new Animation();

            tempIdleAnimation.Initialize(animationSpriteSheets[0], Position, widthHeight, widthHeight,
                animationSpriteSheets[0].Width / widthHeight, AnimationTypes.Idle, scale: Scale);
            
            tempWalkAnimation.Initialize(animationSpriteSheets[1], Position, widthHeight, widthHeight,
                animationSpriteSheets[1].Width / widthHeight, AnimationTypes.Movement, scale: Scale);
            
            tempHurtAnimation.Initialize(animationSpriteSheets[2], Position, widthHeight, widthHeight,
                animationSpriteSheets[2].Width / widthHeight, AnimationTypes.TakingDamage, scale: Scale);
            
            tempDeathAnimation.Initialize(animationSpriteSheets[3], Position, widthHeight, widthHeight
                , animationSpriteSheets[3].Width / widthHeight, AnimationTypes.Death, scale: Scale);


            Animations = new List<Animation>
                {tempIdleAnimation, tempWalkAnimation, tempHurtAnimation, tempDeathAnimation};

            CharacterAnimation = Animations[0];

            Hitbox = new Hitbox(Position, CharacterAnimation.FrameWidth * Scale, CharacterAnimation.FrameHeight * Scale, ColliderType.Enemy);

            InitializeFsm();
        }
        
        protected abstract void InitializeFsm();
        
        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            // Handle behaviour
            Sense(gameTime);
            Think(gameTime);
            
            CharacterAnimation.FacingLeft = Direction.X > 0; 
            CharacterAnimation.Position = Position;
            CharacterAnimation.Update(gameTime);
            
            // Update hitbox
            Hitbox.Position = Position;
            
            // Check damage timer
            DamageCooldown(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // This means it's not active as well as having executed the death animation
            // there's a problem and the death animation doesn't show
            if (!Active && CharacterAnimation == Animations.Find(animation => animation.AnimationType == AnimationTypes.Death) && CanChangeAnimation) return;
            CharacterAnimation?.Draw(spriteBatch);
        }

        
        // Draw imaginary aggro box around enemy and check if player is inside
        protected bool CheckPlayerInRange() 
            => PlayerPosition.X >= Position.X - _visionRange 
               && PlayerPosition.X <= Position.X + _visionRange 
               && PlayerPosition.Y <= Position.Y + _visionRange 
               && PlayerPosition.Y >= Position.Y - _visionRange;

        protected abstract void Sense(GameTime gameTime);
        
        private void Think(GameTime gameTime) => Fsm.Update(gameTime); 
    }
}