﻿using System;
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
        public Texture2D SpriteSheet;
        protected Rectangle SpriteSheetCoordinates;

        public Vector2 Position;
        public bool Active;
        
        
        public Hitbox Hitbox;
        
        // Respawn timer
        public TimeSpan SpawnTime;
        public TimeSpan LastGone;

        public int Width;
        public const int Height = 12; // They all have the same height

        public PickupController PickupController;

        private void Initialize(Texture2D spritesheet)
        {
            SpriteSheet = spritesheet;
            Position = GeneratePickupPosition();
            Active = false;
            Hitbox = new Hitbox(Position, Width, Height, ColliderType.Pickup);
            
            PickupController = new PickupController();
            AddController(PickupController);
        }
        
        public void Update(GameTime gameTime)
        {
            // Check respawn time
            if (!Active && gameTime.TotalGameTime - LastGone > SpawnTime)
                SpawnPickup();
            Hitbox.Position = Position;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            
            spriteBatch.Draw(SpriteSheet, Position, SpriteSheetCoordinates, 
                Color.White, 0f, Vector2.Zero, 2f, 
                SpriteEffects.None, 0f);
        }

        // Returns all initialized pickup types
        public static List<BasePickup> InitAllPickups(Texture2D pickupsSpritesheet)
        {
            var aP = new AmmoPickup();
            aP.Initialize(pickupsSpritesheet);
            var hP = new HealthPickup();
            hP.Initialize(pickupsSpritesheet);
            var fP = new FridgePickup();
            fP.Initialize(pickupsSpritesheet);
            var sP = new SurprisePickup();
            sP.Initialize(pickupsSpritesheet);
            
            return new List<BasePickup> {aP, hP, fP, sP};
        }
        
        // This function "spawns" any given pickup
        private void SpawnPickup()
        {
            Position = GeneratePickupPosition();
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

        public void OnChanged(object sender, PickupEventArgs args)
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