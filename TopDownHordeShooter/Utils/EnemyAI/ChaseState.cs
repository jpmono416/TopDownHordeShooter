using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Utils.EnemyAI
{
    public class ChaseState : State
    {
        public ChaseState()
        {
            Name = "Chase";
        }
        public override void Enter(object owner)
        {
            if (!(owner is WalkerEnemy enemy)) return;
            
            // Set direction 
            enemy.Direction = enemy.PlayerPosition - enemy.Position;
            enemy.Direction.Normalize();

            /* EnemyShip ship = owner as EnemyShip;
             if (ship != null)
             {
                 ship.VelocityScalar = 5000.0f;
                 SelectTarget(ship);
             }*/
        }
        public override void Exit(object owner) 
        {
            if (!(owner is WalkerEnemy enemy)) return;
            enemy.Direction = Vector2.Zero;
            
            /*EnemyShip ship = owner as EnemyShip;
            if (ship != null)
            {
                ship.VelocityScalar = 5000.0f;
                ship.Target = null;
            }*/
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            if (!(owner is WalkerEnemy enemy) || enemy.Colliding) return;

            // Set direction 
            enemy.Direction = enemy.PlayerPosition - enemy.Position;
            enemy.Direction = Vector2.Normalize(enemy.Direction);
            
            enemy.Position +=  enemy.Direction * enemy.MoveSpeed * enemy.SpeedMultiplier;
        }
 
    }
}