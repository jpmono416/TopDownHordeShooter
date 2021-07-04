using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Combat.Projectiles;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.EnemyAI;
using TopDownHordeShooter.Utils.GameState;

namespace TopDownHordeShooter.Entities.Characters
{
    public class ShooterEnemy : Enemy
    {
        // This is the area close to the player where the enemy can't get inside
        private readonly Hitbox _safeArea;
        public readonly Player Player;
        private readonly TimeSpan _secondsBetweenShots;
        public TimeSpan LastTimeShot;
        private bool _inSafeArea;
        public bool CanShoot;
        public readonly List<RedProjectile> ActiveProjectiles;
        public readonly Texture2D ProjectilesSpriteSheet;

        protected override void InitializeFsm()
        {
            Fsm = new Fsm(this);
            
            // Create the states
            var idle = new IdleState();
            var chase = new ChaseState();
            var shoot = new ShootState();

            // Create the transitions between the states
            idle.AddTransition(new Transition(chase, () => !_inSafeArea));
            idle.AddTransition(new Transition(shoot, () => CanShoot));
            chase.AddTransition(new Transition(idle, () => _inSafeArea));
            chase.AddTransition(new Transition(shoot, () => CanShoot));
            shoot.AddTransition(new Transition(chase, () => !_inSafeArea && !CanShoot));
            shoot.AddTransition(new Transition(idle, () => _inSafeArea && !CanShoot));
            
            // Add the created states to the FSM
            Fsm.AddState(idle);
            Fsm.AddState(chase);
            Fsm.AddState(shoot);

            // Set the starting state of the FSM
            Fsm.Initialise("Idle");
        }

        protected override void Sense(GameTime gameTime)
        {
            _inSafeArea = Hitbox.Collides(_safeArea);
            CanShoot = gameTime.TotalGameTime - LastTimeShot >= _secondsBetweenShots;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            PlayerPosition = Player.Position;
            _safeArea.Position = PlayerPosition;
            
            // Projectiles
            DeactivateProjectiles();
            foreach (var proj in ActiveProjectiles) proj.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var proj in ActiveProjectiles)
            {
                proj.Draw(spriteBatch);
            }
        }

        public ShooterEnemy(EnemyData data, DifficultyData difficulty, Player player, Texture2D projectilesSpriteSheet) : base(data, difficulty)
        {
            Scale = 2f;
            SpeedMultiplier = 0.3f;
            Player = player;
            _inSafeArea = false;
            _safeArea = new Hitbox(player.Position, player.CharacterAnimation.FrameWidth * 3, player.CharacterAnimation.FrameHeight * 3, ColliderType.PlayerSafeZone);
            _secondsBetweenShots = TimeSpan.FromSeconds(5);
            ProjectilesSpriteSheet = projectilesSpriteSheet;
            ActiveProjectiles = new List<RedProjectile>();
        }
        
        private void DeactivateProjectiles()
        {
            var tempToBeDeleted = ActiveProjectiles.Where(projectile => !projectile.Active).ToList();
            foreach (var temp in tempToBeDeleted) ActiveProjectiles.Remove(temp); 
        }
    }
}