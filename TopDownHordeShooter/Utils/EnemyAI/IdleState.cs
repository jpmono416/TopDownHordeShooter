using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;

namespace TopDownHordeShooter.Utils.EnemyAI
{
    public class IdleState : State
    {
        public IdleState()
        {
            Name = "Idle";
        }
        public override void Enter(object owner)
        {
            if (!(owner is Enemy enemy)) return;
            
            enemy.Direction = Vector2.Zero;
        }
        public override void Exit(object owner)
        {
            // Nothing happens
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            if (!(owner is Enemy enemy)) return;

            // Change animation
            enemy.ChangeAnimation(gameTime, enemy.Animations.Find(animation => animation.AnimationType == AnimationTypes.Idle));

        } 
    }
}