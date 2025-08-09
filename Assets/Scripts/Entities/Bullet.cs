using TopDownShooter.Core;
using UnityEngine;

namespace TopDownShooter.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class Bullet : MonoBehaviour
    {
        public float Speed;
        public float Lifetime = 2.5f;
        public int Damage = 1;

        private float _timer;
        private Rigidbody2D _rb;
        private ObjectPool<Bullet> _pool;

        public void Init(ObjectPool<Bullet> pool) => _pool = pool;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.12f;
        }

        public void Fire(Vector2 position, Vector2 dir, float speed)
        {
            transform.position = position;
            _rb.velocity = dir.normalized * speed;
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Lifetime)
                _pool.Release(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<EnemyController>(out var enemy))
            {
                enemy.TakeDamage(Damage);
                _pool.Release(this);
            }
        }
    }
}
