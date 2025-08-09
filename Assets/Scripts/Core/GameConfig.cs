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
        public float enemySpeed = 3.5f;
        public float enemySpawnInterval = 1.2f;
        public int   enemyHP = 2;

        [Header("Waves")]
        public int maxEnemiesOnField = 20;

        [Header("Drops")]
        public float powerupDropChance = 0.15f;

        [Header("Gameplay")]
        public Vector2 arenaSize = new Vector2(24f, 14f); // world units (width x height)
    }
}
