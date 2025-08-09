using TopDownShooter.Entities;
using UnityEngine;

namespace TopDownShooter.Systems
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class PowerUp : MonoBehaviour
    {
        public PowerUpType Type = PowerUpType.Spread;

        void Awake()
        {
            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true; col.radius = 0.35f;

            // color by type
            var sr = GetComponent<SpriteRenderer>();
            sr.color = Type switch
            {
                PowerUpType.Heal => new Color(0.5f, 1f, 0.6f),
                PowerUpType.Speed => new Color(0.5f, 0.8f, 1f),
                PowerUpType.Shield => new Color(0.8f, 0.6f, 1f),
                _ => new Color(1f, 0.95f, 0.4f)
            };
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var player)) return;

            switch (Type)
            {
                case PowerUpType.Heal:   player.Heal(2); break;
                case PowerUpType.Speed:  player.MoveSpeed *= 1.25f; Invoke(nameof(ResetSpeed), 8f); break;
                case PowerUpType.Shield: player.UpgradeMK(); break; // simple: treat as upgrade
                case PowerUpType.Spread: player.UpgradeMK(); break;
            }
            Destroy(gameObject);

            void ResetSpeed() { if (player != null) player.MoveSpeed /= 1.25f; }
        }
    }
}
