using TopDownShooter.Core;
using UnityEngine;

namespace TopDownShooter.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        public int MaxHP = 5;
        public float MoveSpeed = 7f;
        public float FireCooldown = 0.12f;
        public float BulletSpeed = 18f;

        public int HP { get; private set; }
        public int MKLevel { get; private set; } = 1;

        private Rigidbody2D _rb;
        private Camera _cam;
        private float _fireTimer;
        private ObjectPool<Bullet> _bulletPool;

        public System.Action<int, int> OnHPChanged; // (hp,max)
        public System.Action<int> OnMKChanged;      // level

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _cam = Camera.main;
            var col = GetComponent<CapsuleCollider2D>();
            col.isTrigger = true;
            col.direction = CapsuleDirection2D.Vertical;
            col.size = new Vector2(0.6f, 1.1f);
        }

        public void Init(GameConfig cfg, ObjectPool<Bullet> bulletPool)
        {
            MaxHP = cfg.playerMaxHP;
            MoveSpeed = cfg.playerSpeed;
            FireCooldown = cfg.fireCooldown;
            BulletSpeed = cfg.bulletSpeed;

            HP = MaxHP;
            OnHPChanged?.Invoke(HP, MaxHP);
            _bulletPool = bulletPool;
        }

        private void Update()
        {
            // Movement
            var x = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
                  - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)  ? 1 : 0);
            var y = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)    ? 1 : 0)
                  - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)  ? 1 : 0);
            var v = new Vector2(x, y);
            if (v.sqrMagnitude > 1f) v.Normalize();
            _rb.velocity = v * MoveSpeed;

            // Aim to mouse
            var mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            var dir = (mouseWorld - transform.position);
            dir.z = 0;
            if (dir.sqrMagnitude > 0.001f)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            // Shooting
            _fireTimer -= Time.deltaTime;
            if (Input.GetMouseButton(0) && _fireTimer <= 0f)
            {
                Shoot(dir.normalized);
                _fireTimer = FireCooldown;
            }
        }

        private void Shoot(Vector2 dir)
        {
            // MK levels fire patterns
            if (MKLevel <= 1) FireOne(dir, 0f);
            else if (MKLevel == 2) { FireOne(dir, -6f); FireOne(dir, 6f); }
            else { FireOne(dir, -10f); FireOne(dir, 0f); FireOne(dir, 10f); }
        }

        private void FireOne(Vector2 dir, float spreadDeg)
        {
            var rad = spreadDeg * Mathf.Deg2Rad;
            var spreadDir = new Vector2(
                dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad),
                dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad)
            );

            var bullet = _bulletPool.Get();
            bullet.Init(_bulletPool);
            bullet.transform.position = transform.position + (Vector3)(spreadDir * 0.8f);
            bullet.GetComponent<SpriteRenderer>().color = new Color(1f, 0.95f, 0.3f, 1f);
            bullet.Fire(bullet.transform.position, spreadDir, BulletSpeed);
        }

        public void TakeDamage(int dmg)
        {
            HP = Mathf.Max(0, HP - dmg);
            OnHPChanged?.Invoke(HP, MaxHP);
        }

        public void Heal(int amount)
        {
            HP = Mathf.Min(MaxHP, HP + amount);
            OnHPChanged?.Invoke(HP, MaxHP);
        }

        public void UpgradeMK()
        {
            MKLevel = Mathf.Clamp(MKLevel + 1, 1, 3);
            OnMKChanged?.Invoke(MKLevel);
        }
    }
}
