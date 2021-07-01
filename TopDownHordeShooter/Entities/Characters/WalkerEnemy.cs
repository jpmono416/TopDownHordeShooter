using TopDownHordeShooter.Utils;
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
            //FleeState flee = new FleeState();
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

        public WalkerEnemy(EnemyData data) : base(data)
        {
            SpeedMultiplier = 0.1f;
        }
    }
}