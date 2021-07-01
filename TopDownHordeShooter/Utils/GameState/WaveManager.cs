using System;
using Microsoft.Xna.Framework;

namespace TopDownHordeShooter.Utils.GameState
{
    public class WaveManager
    {
        public int CurrentWave;
        private readonly TimeSpan DelayBetweenWaves;
        private TimeSpan TimeWaveCompleted;
        
        // True when the current wave is over
        public bool WaveCompleted;
        
        // True when new wave can begin after delay
        public bool TimerOver;

        public WaveManager()
        {
            WaveCompleted = false;
            CurrentWave = 1;
            DelayBetweenWaves = TimeSpan.FromSeconds(5);
        }

        public void Update(GameTime gameTime)
        {
            if (!WaveCompleted) return;

            //TimeWaveCompleted = gameTime.TotalGameTime;

            //if (gameTime.TotalGameTime - TimeWaveCompleted <= DelayBetweenWaves) return;

            WaveCompleted = false;
            CurrentWave++;

            // For respawn purposes
            TimerOver = true;
            
            // Prevent this to be triggered during a wave 
            //TimeWaveCompleted = TimeSpan.MaxValue;
        }
    }
}