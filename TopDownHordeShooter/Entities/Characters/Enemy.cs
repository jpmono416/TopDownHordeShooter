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

        // Enemy direction facing towards player, calculated on the spot but needs to be normalized before use

        protected Enemy(EnemyData data)
        {
            Health = data.Health;
            BaseDamage = data.Damage;
            MoveSpeed = data.MoveSpeed;
            _visionRange = data.VisionRange;
            ScoreGiven = data.ScoreGiven;
            PlayerInRange = false;
            CanTakeDamage = true;
            Animations = new List<Animation>();
            CharacterAnimation = new Animation();
            
            ReceiveDamageTimeSpan = TimeSpan.FromMilliseconds(500);
            LastDamageReceived = TimeSpan.Zero;
            
            Active = true;
        }
        
        public override void Initialize(List<Texture2D> animationSpriteSheets, Vector2 position)
        {
            Position = position;
            foreach (var spriteSheet in animationSpriteSheets)
            {
                var tempAnimation = new Animation();
                tempAnimation.Initialize(spriteSheet, Position, 64, 64, 7, 30, Color.White, 1, true);
                
                Animations.Add(tempAnimation);
            }
            CharacterTexture = animationSpriteSheets[0]; // Irrelevant, not in use
            CharacterAnimation = Animations[0];

            Hitbox = new Hitbox(Position, CharacterAnimation.FrameWidth, CharacterAnimation.FrameHeight, ColliderType.Enemy);

            InitializeFsm();
        }
        
        protected abstract void InitializeFsm();
        
        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            // Handle behaviour
            Sense();
            Think(gameTime);
            
            CharacterAnimation.Position = Position;
            CharacterAnimation.Update(gameTime);
            
            // Update hitbox
            Hitbox.Position = Position;
            
            // Check damage timer
            DamageCooldown(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            CharacterAnimation.Draw(spriteBatch);
        }

        
        // Draw imaginary aggro box around enemy and check if player is inside
        private bool CheckPlayerInRange() 
            => PlayerPosition.X >= Position.X - _visionRange 
               && PlayerPosition.X <= Position.X + _visionRange 
               && PlayerPosition.Y <= Position.Y + _visionRange 
               && PlayerPosition.Y >= Position.Y - _visionRange;

        private void Sense() => PlayerInRange = CheckPlayerInRange();
        
        private void Think(GameTime gameTime) => Fsm.Update(gameTime); 
    }
}