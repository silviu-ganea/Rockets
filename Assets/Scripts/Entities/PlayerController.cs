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
        public float MuzzleOffset = 1.0f;    // distance from ship center to nose (world units)
        public float LateralSpacing = 0.25f; // sideways offset for spread

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

            // MuzzleOffset/LateralSpacing defaults that work for most ships
            if (MuzzleOffset <= 0f) MuzzleOffset = 1.0f;
            if (LateralSpacing <= 0f) LateralSpacing = 0.25f;
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
#if ENABLE_INPUT_SYSTEM
            // New Input System polling (no actions asset required)
            var kb = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;

            float x = 0f, y = 0f;
            if (kb != null) {
                x = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1 : 0)
                  - (kb.aKey.isPressed || kb.leftArrowKey.isPressed  ? 1 : 0);
                y = (kb.wKey.isPressed || kb.upArrowKey.isPressed    ? 1 : 0)
                  - (kb.sKey.isPressed || kb.downArrowKey.isPressed  ? 1 : 0);
            }
            var v = new Vector2(x, y);
            if (v.sqrMagnitude > 1f) v.Normalize();
            _rb.linearVelocity = v * MoveSpeed;

            Vector3 mouseScreen = mouse != null ? (Vector3)mouse.position.ReadValue() : Input.mousePosition;
            var mouseWorld = _cam.ScreenToWorldPoint(mouseScreen);
            var dir = (mouseWorld - transform.position);
            dir.z = 0;
            if (dir.sqrMagnitude > 0.001f)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            _fireTimer -= Time.deltaTime;
            bool fire = mouse != null ? mouse.leftButton.isPressed : Input.GetMouseButton(0);
            if (fire && _fireTimer <= 0f)
            {
                Shoot(dir.normalized);
                _fireTimer = FireCooldown;
            }
#else
            // Old Input Manager
            var x = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
                  - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)  ? 1 : 0);
            var y = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)    ? 1 : 0)
                  - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)  ? 1 : 0);
            var v = new Vector2(x, y);
            if (v.sqrMagnitude > 1f) v.Normalize();
            _rb.velocity = v * MoveSpeed;

            var mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            var dir = (mouseWorld - transform.position);
            dir.z = 0;
            if (dir.sqrMagnitude > 0.001f)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            _fireTimer -= Time.deltaTime;
            if (Input.GetMouseButton(0) && _fireTimer <= 0f)
            {
                Shoot(dir.normalized);
                _fireTimer = FireCooldown;
            }
#endif
        }

        
        private void Shoot(Vector2 dir)
        {
            var fwd = dir.normalized;
            if (MKLevel <= 1)
            {
                FireOne(fwd, 0f);
            }
            else if (MKLevel == 2)
            {
                FireOne(fwd, -6f);
                FireOne(fwd,  6f);
            }
            else
            {
                FireOne(fwd, -10f);
                FireOne(fwd,   0f);
                FireOne(fwd,  10f);
            }
        }

        
        private void FireOne(Vector2 fwd, float spreadDeg)
        {
            // right vector perpendicular to forward
            var right = new Vector2(-fwd.y, fwd.x);

            float rad = spreadDeg * Mathf.Deg2Rad;

            // rotate forward by spread
            var dir = new Vector2(
                fwd.x * Mathf.Cos(rad) - fwd.y * Mathf.Sin(rad),
                fwd.x * Mathf.Sin(rad) + fwd.y * Mathf.Cos(rad)
            ).normalized;

            // spawn at the NOSE, plus a small lateral offset for side shots
            float lateral = Mathf.Sin(rad) * LateralSpacing;
            Vector3 spawnPos = transform.position + (Vector3)(fwd * MuzzleOffset + right * lateral);

            var bullet = _bulletPool.Get();
            bullet.Init(_bulletPool);
            bullet.transform.position = spawnPos;
            var sr = bullet.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 0.95f, 0.3f, 1f);
            bullet.Fire(spawnPos, dir, BulletSpeed);
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
