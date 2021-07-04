using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TopDownHordeShooter.Entities.Combat.Projectiles;

namespace TopDownHordeShooter.Entities.Combat.Weapons
{
    public abstract class BaseWeapon
    {
        // Textures
        public bool WeaponFacingLeft;
        private Texture2D _weaponTexture;
        public Vector2 PlayerPosition;
        protected Rectangle SpriteSheetCoordinates;
        private Vector2 _drawPosition;

        // This alters the player's base stats
        public int DamageOffset;
        public float RateOfFireOffset;

        // Bullets and mags
        public int CurrentBulletsInMag;
        public int RemainingBullets; // Apart from the ones in the mag
        public int MaxBulletsPerMag;
        protected TimeSpan ProjectileDuration;
        
        
        // Reloading
        private TimeSpan _reloadTime;
        private TimeSpan _lastReloaded;
        private bool _reloading;
        
        // Whether the weapon has been unlocked to use
        public bool Unlocked;
        protected int LevelToUnlock;

        // Weapon currently equipped?
        public bool Active;

        public List<BaseProjectile> ActiveProjectiles;
        
        // Shoot sound
        private List<SoundEffect> _allEffects;
        protected SoundEffect ShootSound;
        protected SoundEffectInstance ShootSoundInstance;

        public static List<BaseWeapon> InitAllWeapons(Texture2D weaponsSpritesheet, List<SoundEffect> soundEffects)
        {
            
            // Create objects
            var assaultRifle = new AssaultRifle();
            var beretta = new Beretta();
            var sniperRifle = new SniperRifle();
            var flamethrower = new Flamethrower();
            //var minigun = new Minigun();
            //var lightSaber = new LightSaber(); no time to implement melee
            //var rpg = new Rpg(); No time to implement detaching rocket head and explosion
            //var shotgun = new Shotgun(); No time to implement pellet spread with animation and col.det.

            beretta.ShootSound = soundEffects[0];
            assaultRifle.ShootSound = soundEffects[1];
            sniperRifle.ShootSound = soundEffects[2];
            flamethrower.ShootSound = soundEffects[3];
            
            var weapons = new List<BaseWeapon>
            {
                assaultRifle,
                beretta,
                sniperRifle,
                flamethrower,
                //minigun
                //lightSaber,
                //rpg,
                //shotgun,
                
            };
            // List building

            // Initialization
            
            foreach (var wpn in weapons) wpn.Initialize(weaponsSpritesheet);
            
            return weapons;
        }

        public void UpdateUnlocked(int playerLevel) => Unlocked = LevelToUnlock <= playerLevel;

        private void Initialize(Texture2D texture) => _weaponTexture = texture;

        public void Update(GameTime gameTime)
        {
            if (!Unlocked || !Active) return;

            // Finish reload timer
            if (_reloading && _lastReloaded + _reloadTime <= gameTime.TotalGameTime) 
                _reloading = false;

            foreach (var projectile in ActiveProjectiles) projectile.Update(gameTime); 
        }

        public void Shoot(GameTime gameTime, bool left, Texture2D bulletsSpritesheet)
        {
            if(_reloading || RemainingBullets == 0 && CurrentBulletsInMag == 0) return;

            var rightNormal = new Vector2(1f, 0f);
            var leftNormal = new Vector2(-1f, 0f);
            var direction = left ? leftNormal : rightNormal;

            var tempProjectile = new BlueProjectile(bulletsSpritesheet) {SpawnTime = gameTime.TotalGameTime};
            tempProjectile.Initialize(_drawPosition, direction, ProjectileDuration);
            
            ActiveProjectiles.Add(tempProjectile);
            
            // Sound
            ShootSoundInstance = ShootSound.CreateInstance();
            ShootSoundInstance.Play();
            
            CurrentBulletsInMag -= 1;
            if(CurrentBulletsInMag == 0) Reload(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Unlocked || !Active) return;
            
            _drawPosition = !WeaponFacingLeft ? new Vector2(PlayerPosition.X + 15f, PlayerPosition.Y + 32f) : 
                    new Vector2(PlayerPosition.X - 15f, PlayerPosition.Y + 32f);
            
            spriteBatch.Draw(_weaponTexture, _drawPosition, SpriteSheetCoordinates, 
                Color.White, 0f, Vector2.Zero, 1f, 
                !WeaponFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            foreach (var projectile in ActiveProjectiles) projectile.Draw(spriteBatch);
        }

        public void Reload(GameTime gameTime)
        {
            if(_reloading || CurrentBulletsInMag == MaxBulletsPerMag || RemainingBullets == 0) return;
            
            _reloading = true;
            _lastReloaded = gameTime.TotalGameTime;

            // Change magazines and update bullet count
            RemainingBullets += CurrentBulletsInMag - MaxBulletsPerMag;
            CurrentBulletsInMag = MaxBulletsPerMag;
        }
    }
}