using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Characters;
using TopDownHordeShooter.Utils;
using TopDownHordeShooter.Utils.Events;

namespace TopDownHordeShooter.Entities.Pickups
{
    public abstract class BasePickup
    {
        //Texture
        private Texture2D _spriteSheet;
        protected Rectangle SpriteSheetCoordinates;

        private Vector2 _position;
        public bool Active;
        
        
        public Hitbox Hitbox;
        
        // Respawn timer
        protected TimeSpan SpawnTimeSpan;
        public TimeSpan LastGone;

        protected int Width;
        protected const int Height = 12; // They all have the same height

        public PickupController PickupController;

        private void Initialize(Texture2D spritesheet)
        {
            _spriteSheet = spritesheet;
            _position = GeneratePickupPosition();
            Active = false;
            Hitbox = new Hitbox(_position, Width, Height, ColliderType.Pickup);
            
            PickupController = new PickupController();
            AddController(PickupController);
        }
        
        public void Update(GameTime gameTime)
        {
            // Check respawn time
            if (!Active && gameTime.TotalGameTime - LastGone > SpawnTimeSpan)
                SpawnPickup();
            Hitbox.Position = _position;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            
            spriteBatch.Draw(_spriteSheet, _position, SpriteSheetCoordinates, 
                Color.White, 0f, Vector2.Zero, 2f, 
                SpriteEffects.None, 0f);
        }

        // Returns all initialized pickup types
        public static List<BasePickup> InitAllPickups(Texture2D pickupsSpritesheet, int basePickupSpawnTime)
        {
            var aP = new AmmoPickup(basePickupSpawnTime);
            aP.Initialize(pickupsSpritesheet);
            var hP = new HealthPickup(basePickupSpawnTime);
            hP.Initialize(pickupsSpritesheet);
            var fP = new FridgePickup(basePickupSpawnTime);
            fP.Initialize(pickupsSpritesheet);
            var sP = new SurprisePickup(basePickupSpawnTime);
            sP.Initialize(pickupsSpritesheet);
            
            return new List<BasePickup> {aP, hP, fP, sP};
        }
        
        // This function "spawns" any given pickup
        private void SpawnPickup()
        {
            _position = GeneratePickupPosition();
            Active = true;
        }

        protected abstract void ApplyEffect(Player player, GameTime gameTime);

        private static Vector2 GeneratePickupPosition()
        {
            var random = new Random();
            return new Vector2(random.Next(0, 1920), random.Next(0, 1080));
        }
        
        private void AddController(PickupController pickupController) =>
            pickupController.Changed += OnChanged;

        private void OnChanged(object sender, PickupEventArgs args)
        {
            switch (args.EventType)
            {
                case PickupEventType.Spawn:
                    SpawnPickup(); //TODO check for spawn condition on main
                    break;

                case PickupEventType.ApplyEffect:
                    ApplyEffect(args.Player, args.GameTime);
                    break;
            }
        }
    }
}