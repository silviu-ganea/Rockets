using TopDownShooter.Entities;
using UnityEngine;

namespace TopDownShooter.Systems
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class PowerUp : MonoBehaviour
    {
        private void Awake()
        {
            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.UpgradeMK();
                Destroy(gameObject);
            }
        }
    }
}
