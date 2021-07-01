using System.Collections.Generic;

namespace TopDownHordeShooter.Utils
{
    public class GameData
    {
        public PlayerStats PlayerStats{ get; set; }
        public EnemyData EnemyData{ get; set; }
        public DifficultyData DifficultyData{ get; set; }

        public GameData(PlayerStats playerStats, EnemyData enemyData, DifficultyData difficultyData)
        {
            PlayerStats = playerStats;
            EnemyData = enemyData;
            DifficultyData = difficultyData;
        }
    }
    
    public class EnemyData
    {
        public int Health { get; set; }
        public float MoveSpeed{ get; set; }
        public int Damage{ get; set; }
        public int ScoreGiven{ get; set; }
        public float VisionRange{ get; set; }

        public EnemyData(int health, float moveSpeed, int damage, int scoreGiven, float visionRange)
        {
            Health = health;
            MoveSpeed = moveSpeed;
            Damage = damage;
            ScoreGiven = scoreGiven;
            VisionRange = visionRange;
        }
        public EnemyData(){}
    }

    public class PlayerStats
    {
        public int Level{ get; set; }
        public int Health{ get; set; }
        public float BaseMoveSpeed{ get; set; }
        public int BaseDamage{ get; set; }
        public float BaseFireRate{ get; set; } // Bullets per minute

        public PlayerStats(int level, int health, float moveSpeed, int baseDamage, float fireRate)
        {
            Level = level;
            Health = health;
            BaseMoveSpeed = moveSpeed;
            BaseDamage = baseDamage;
            BaseFireRate = fireRate;
        }

        public PlayerStats(){}
    }

    public class DifficultyData
    {
        public int Level{ get; set; }
        public float EnemyDamageMultiplier{ get; set; }
        public int BasePickupSpawnTime{ get; set; } // In seconds
        public int MinimumEnemiesPerWave{ get; set; }

        public DifficultyData(int level, float damageMultiplier, int pickupSpawnTime, int minEnemies)
        {
            Level = level;
            EnemyDamageMultiplier = damageMultiplier;
            BasePickupSpawnTime = pickupSpawnTime;
            MinimumEnemiesPerWave = minEnemies;
        }

        public DifficultyData(){}
    }

    //TODO use this
    public class HighScores
    {
        public List<int> Scores { get; set; }
        public void SortList() => Scores.Sort();
        
        public HighScores(List<int> scores) => Scores = scores;
        public HighScores(){}
    }
}