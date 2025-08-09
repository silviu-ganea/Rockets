using UnityEngine;

namespace TopDownShooter.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class EnemyController : MonoBehaviour
    {
        public int MaxHP = 2;
        public float Speed = 3f;

        private int _hp;
        private Rigidbody2D _rb;
        private Transform _target;
        private TopDownShooter.Spawner _spawner;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
        }

        public void Init(Transform target, TopDownShooter.Spawner spawner, int hp, float speed)
        {
            _target = target;
            _spawner = spawner;
            _hp = hp;
            Speed = speed;
        }

        private void Update()
        {
            if (_target == null) return;
            var dir = (_target.position - transform.position).normalized;
            _rb.linearVelocity = dir * Speed;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        public void TakeDamage(int dmg)
        {
            _hp -= dmg;
            if (_hp <= 0)
            {
                _spawner.OnEnemyKilled(this, transform.position);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.TakeDamage(1);
                _spawner.OnEnemyKilled(this, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
