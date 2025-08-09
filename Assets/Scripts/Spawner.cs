using TopDownShooter.Core;
using TopDownShooter.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TopDownShooter
{
    public class Spawner : MonoBehaviour
    {
        public GameConfig Config;
        public Transform Player;
        public GameObject EnemyPrefab;
        public GameObject PowerUpPrefab;

        private float _timer;

        private void Update()
        {
            if (Player == null || EnemyPrefab == null) return;

            _timer += Time.deltaTime;
            if (_timer >= Config.enemySpawnInterval)
            {
                _timer = 0f;
                var existing = FindObjectsOfType<EnemyController>().Length;
                if (existing < Config.maxEnemiesOnField)
                    SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            var halfW = Config.arenaSize.x * 0.5f;
            var halfH = Config.arenaSize.y * 0.5f;

            // random edge spawn
            var edge = Random.Range(0, 4);
            Vector2 pos = edge switch
            {
                0 => new Vector2(-halfW, Random.Range(-halfH, halfH)),
                1 => new Vector2(halfW, Random.Range(-halfH, halfH)),
                2 => new Vector2(Random.Range(-halfW, halfW), halfH),
                _ => new Vector2(Random.Range(-halfW, halfW), -halfH),
            };

            var enemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
            enemy.SetActive(true);
            var ec = enemy.GetComponent<EnemyController>();
            ec.Init(Player, this, Config.enemyHP, Config.enemySpeed);
        }

        public void OnEnemyKilled(EnemyController enemy, Vector3 where)
        {
            if (Random.value < Config.powerupDropChance)
            {
                Instantiate(PowerUpPrefab, where, Quaternion.identity);
            }
        }
    }
}
