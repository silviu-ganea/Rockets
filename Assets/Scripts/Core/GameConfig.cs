using UnityEngine;

namespace TopDownShooter.Core
{
    [CreateAssetMenu(menuName = "TopDownShooter/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Player")]
        public float playerSpeed = 7f;
        public float bulletSpeed = 18f;
        public float fireCooldown = 0.12f;
        public int   playerMaxHP = 5;

        [Header("Enemies")]
        public float enemySpeed = 4.2f;          // slightly faster across bigger map
        public float enemySpawnInterval = 0.9f;  // a bit quicker to keep density
        public int   enemyHP = 2;

        [Header("Waves")]
        public int maxEnemiesOnField = 35;

        [Header("Drops")]
        public float powerupDropChance = 0.15f;

        [Header("Gameplay")]
        public Vector2 arenaSize = new Vector2(72f, 42f); // ~3x previous (24x14)
        [Tooltip("Enemies must spawn at least this far from the player")]
        public float minEnemySpawnDistance = 12f;
    }
}
