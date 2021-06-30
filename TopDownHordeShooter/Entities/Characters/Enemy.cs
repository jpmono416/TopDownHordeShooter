using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.EnemyAI;

namespace TopDownHordeShooter.Entities.Characters
{
    public abstract class Enemy : BaseCharacter
    {
        
        // The amount of score the enemy will give to the player
        public int ScoreGiven;

        // FSM for actions
        protected Fsm Fsm;
        
        // Sensor for AI
        protected float VisionRange;

        // Store player position for quicker and simpler AI calculations. Should be OK on a PvE game
        public Vector2 PlayerPosition { get; set; }
        public bool PlayerInRange;

        // Enemy direction facing towards player, calculated on the spot but needs to be normalized before use

        public Enemy(EnemyData data)
        {
            Health = data.Health;
            BaseDamage = data.Damage;
            MoveSpeed = data.MoveSpeed;
            Active = true;
            VisionRange = data.VisionRange;
            ScoreGiven = data.ScoreGiven;
            PlayerInRange = false;
            CanTakeDamage = true;
        }
        
        public override void Initialize(Texture2D charTexture, Vector2 position)
        {
            CharacterTexture = charTexture;
            Position = position;

            Hitbox = new Hitbox(Position, Width, Height, ColliderType.Enemy);
            
            
            InitializeFsm();
        }

        protected abstract void InitializeFsm();
        
        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            // Handle behaviour
            Sense();
            Think(gameTime);
            
            // Update hitbox
            Hitbox.Position = Position;
            
            // Check damage timer
            DamageCooldown(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            
            //hitbox.Draw(spriteBatch);
            Vector2 drawPosition;
            drawPosition.X = Position.X - Width/2;
            drawPosition.Y = Position.Y - Height/2;
            
            Hitbox.Draw(spriteBatch);
            
            spriteBatch.Draw(CharacterTexture, Position, null, Color.White, 0f, Vector2.Zero,
                1f, !CharacterFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        
        // Draw imaginary aggro box around enemy and check if player is inside
        private bool CheckPlayerInRange() 
            => PlayerPosition.X >= Position.X - VisionRange 
               && PlayerPosition.X <= Position.X + VisionRange 
               && PlayerPosition.Y <= Position.Y + VisionRange 
               && PlayerPosition.Y >= Position.Y - VisionRange;

        private void Sense()
        {
            PlayerInRange = CheckPlayerInRange(); 
        }
        
        private void Think(GameTime gameTime) => Fsm.Update(gameTime); 
    }
}