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
            enemy.Direction = Vector2.Normalize(enemy.Direction);
        }
        public override void Exit(object owner) 
        {
            if (!(owner is WalkerEnemy enemy)) return;

            enemy.Direction = Vector2.Zero;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            if (!(owner is Enemy enemy)) return;

            // Change animation
            enemy.ChangeAnimation(gameTime, enemy.Animations.Find(animation => animation.AnimationType == AnimationTypes.Movement));
            
            // Set direction 
            enemy.Direction = enemy.PlayerPosition - enemy.Position;
            enemy.Direction = Vector2.Normalize(enemy.Direction);
            
            enemy.Position +=  enemy.Direction * enemy.MoveSpeed * enemy.SpeedMultiplier;
        }
 
    }
}