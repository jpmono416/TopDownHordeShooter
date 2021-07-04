using Microsoft.Xna.Framework;
using TopDownHordeShooter.Utils.EnemyAI;
using TopDownHordeShooter.Utils.GameState;

namespace TopDownHordeShooter.Entities.Characters
{
    public class WalkerEnemy : Enemy
    {
        protected override void InitializeFsm()
        {
            Fsm = new Fsm(this);
            
            // Create the states
            var idle = new IdleState();
            var chase = new ChaseState();
            
            // Create the transitions between the states
            chase.AddTransition(new Transition(idle, () => !PlayerInRange));
            idle.AddTransition(new Transition(chase, () => PlayerInRange));

            // Add the created states to the FSM
            Fsm.AddState(idle);
            Fsm.AddState(chase);

            // Set the starting state of the FSM
            Fsm.Initialise("Idle");
        }

        protected override void Sense(GameTime gameTime) => PlayerInRange = CheckPlayerInRange();
        

        public WalkerEnemy(EnemyData data, DifficultyData difficulty) : base(data, difficulty)
        {
            SpeedMultiplier = 0.1f;
            Scale = 1f;
        }
    }
}