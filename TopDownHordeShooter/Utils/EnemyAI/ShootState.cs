using System;
using Microsoft.Xna.Framework;
using TopDownHordeShooter.Entities.Characters;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Utils.EnemyAI
{
    public class ShootState : State
    {
        public ShootState()
        {
            Name = "Shoot";
        }
        public override void Enter(object owner)
        {
            if (!(owner is ShooterEnemy enemy)) return;
            
            // Set direction 
            enemy.Direction = enemy.PlayerPosition - enemy.Position;
            
            enemy.Direction = Vector2.Normalize(enemy.Direction);
        }
        public override void Exit(object owner) 
        {
            if (!(owner is ShooterEnemy enemy)) return;
            enemy.Direction = Vector2.Zero;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            if (!(owner is ShooterEnemy enemy)) return;

            // Set direction 
            enemy.Direction = enemy.Player.Position - enemy.Position;
            enemy.Direction = Vector2.Normalize(enemy.Direction);
            
            var shot = new RedProjectile(enemy.ProjectilesSpriteSheet);
            shot.Initialize(enemy.Position,enemy.Direction, TimeSpan.FromSeconds(2));
            shot.SpawnTime = gameTime.TotalGameTime;
            enemy.ActiveProjectiles.Add(shot);
            
            enemy.CanShoot = false;
            enemy.LastTimeShot = gameTime.TotalGameTime;
        }
 
    }
}