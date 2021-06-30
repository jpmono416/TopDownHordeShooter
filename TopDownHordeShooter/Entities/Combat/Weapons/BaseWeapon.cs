using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public abstract class BaseWeapon
    {
        // Textures
        public bool WeaponFacingLeft;
        public Texture2D WeaponTexture;
        public Vector2 PlayerPosition;
        protected Rectangle SpriteSheetCoordinates;
        public Vector2 DrawPosition;

        // This alters the player's base stats
        public int DamageOffset;
        public float RateOfFireOffset;

        // Bullets and mags
        public int CurrentBulletsInMag;
        public int RemainingBullets; // Apart from the one in the mag
        public int MaxBulletsPerMag;
        
        
        // Reloading
        public TimeSpan ReloadTime;
        public TimeSpan LastReloaded;
        public bool Reloading;
        
        // Whether the weapon has been unlocked to use
        public bool Unlocked;
        private int _levelToUnlock;

        // Weapon currently equipped?
        public bool Active;

        public List<BaseProjectile> ActiveProjectiles;

        public static List<BaseWeapon> InitAllWeapons(Texture2D weaponsSpritesheet)
        {
            // Create objects
            var assaultRifle = new AssaultRifle();
            var beretta = new Beretta();
            var flamethrower = new Flamethrower();
            var lightSaber = new LightSaber();
            var minigun = new Minigun();
            var rpg = new Rpg();
            var shotgun = new Shotgun();
            var sniperRifle = new SniperRifle();

            // List building
            var weapons = new List<BaseWeapon>
            {
                assaultRifle,
                beretta,
                flamethrower,
                lightSaber,
                minigun,
                rpg,
                shotgun,
                sniperRifle
            };

            // Initialization
            foreach (var wpn in weapons) wpn.Initialize(weaponsSpritesheet);
            
            return weapons;
        }

        public void UpdateUnlocked(int playerLevel)
        {
            if (Unlocked) return;
            if (_levelToUnlock <= playerLevel) Unlocked = true;
        }
        
        public void Initialize(Texture2D texture) => WeaponTexture = texture;

        public void Update(GameTime gameTime)
        {
            //if (!Unlocked || !Active) return;
            
            // Finish reload timer
            if (Reloading && LastReloaded + ReloadTime <= gameTime.TotalGameTime) 
                Reloading = false;

            foreach (var projectile in ActiveProjectiles) projectile.Update(gameTime); 
        }

        public void Shoot(GameTime gameTime, bool left, Texture2D bulletsSpritesheet)
        {
            if(Reloading || RemainingBullets == 0 && CurrentBulletsInMag == 0) return;

            
            var rightNormal = new Vector2(1f, 0f);
            var leftNormal = new Vector2(-1f, 0f);
            var direction = left ? leftNormal : rightNormal;

            var tempProjectile = new BlueProjectile(bulletsSpritesheet) {SpawnTime = gameTime.TotalGameTime};
            tempProjectile.Initialize(DrawPosition, direction);
            
            ActiveProjectiles.Add(tempProjectile);
            
            CurrentBulletsInMag -= 1;
            if(CurrentBulletsInMag == 0) Reload(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            //if (!Unlocked || !Active) return;
            
            DrawPosition = !WeaponFacingLeft ? new Vector2(PlayerPosition.X + 15f, PlayerPosition.Y + 32f) : 
                    new Vector2(PlayerPosition.X - 15f, PlayerPosition.Y + 32f);
            
            spriteBatch.Draw(WeaponTexture, DrawPosition, SpriteSheetCoordinates, 
                Color.White, 0f, Vector2.Zero, 1f, 
                !WeaponFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            foreach (var projectile in ActiveProjectiles) projectile.Draw(spriteBatch);
        }

        public void Reload(GameTime gameTime)
        {
            if(Reloading || CurrentBulletsInMag == MaxBulletsPerMag || RemainingBullets == 0) return;
            
            Reloading = true;
            LastReloaded = gameTime.TotalGameTime;

            // Change magazines and update bullet count
            RemainingBullets += CurrentBulletsInMag - MaxBulletsPerMag;
            CurrentBulletsInMag = MaxBulletsPerMag;
        }
    }
}