using TopDownShooter.Core;
using TopDownShooter.Entities;
using TopDownShooter.UI;
using TopDownShooter.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class Bootstrapper : MonoBehaviour
    {
        // Optional: drag a ScriptableObject GameConfig; if null, a default is created at runtime
        public GameConfig Config;

        // From ArtConfig branch
        public ArtConfig Art;

        // From toggles branch
        public bool SpawnParallax = false;
        public bool DrawArenaLines = false;

        private ObjectPool<Bullet> _bulletPool;
        private HUDController _hud;

        private void Start()
        {
            Application.targetFrameRate = 60;

            // Camera (reuse if exists)
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGO = new GameObject("MainCamera");
                cam = camGO.AddComponent<Camera>();
                cam.tag = "MainCamera";
            }
            cam.transform.position = new Vector3(0f, 0f, -10f);
            var camCtrl = cam.gameObject.GetComponent<TopDownShooter.Rendering.CameraController>();
            if (camCtrl == null) camCtrl = cam.gameObject.AddComponent<TopDownShooter.Rendering.CameraController>();
            camCtrl.OrthoSize = 36f;

            // Arena
            if (DrawArenaLines) CreateArenaLines();

            // Player
            var player = CreatePlayer();
            camCtrl.Target = player.transform;

            // Optional parallax (disabled by default)
            if (SpawnParallax)
            {
                var bg = new GameObject("ParallaxBackground");
                var par = bg.AddComponent<TopDownShooter.Rendering.ParallaxBackground>();
                par.Follow = player.transform;
            }

            // Bullets + pool
            var bulletPrefab = CreateBulletPrefab();
            _bulletPool = new ObjectPool<Bullet>(bulletPrefab, 64, this.transform);
            player.Init(ConfigOrDefault(), _bulletPool);

            // Spawner
            var spawnerGO = new GameObject("Spawner");
            var spawner = spawnerGO.AddComponent<Spawner>();
            spawner.Config = ConfigOrDefault();
            spawner.Player = player.transform;
            spawner.EnemyPrefab = CreateEnemyPrefab();
            spawner.PowerUpPrefab = CreatePowerUpPrefab();

            // HUD
            _hud = CreateHUD();
            _hud.Bind(player);

            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<TopDownShooter.Core.GameManager>();
            gm.Bind(_hud, player);
        }

        private GameConfig ConfigOrDefault()
        {
            if (Config != null) return Config;
            var cfg = ScriptableObject.CreateInstance<GameConfig>();
            return cfg;
        }

        private PlayerController CreatePlayer()
        {
            var go = new GameObject("Player");
            go.transform.position = Vector3.zero;

            var sr = go.AddComponent<SpriteRenderer>();
            if (Art != null && Art.playerShip != null) sr.sprite = Art.playerShip;
            else sr.sprite = MakeCircleSprite(new Color(1f, 0.92f, 0.35f), 64);
            sr.sortingOrder = 10;

            var pc = go.AddComponent<PlayerController>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<CapsuleCollider2D>();

            return pc;
        }

        private GameObject CreateEnemyPrefab()
        {
            var go = new GameObject("EnemyPrefab");
            var sr = go.AddComponent<SpriteRenderer>();
            var enemySprite = TopDownShooter.Core.Art.PickEnemy(Art);
            if (enemySprite != null) sr.sprite = enemySprite;
            else sr.sprite = MakeTriangleSprite(new Color(0.95f, 0.45f, 0.35f), 64);
            sr.sortingOrder = 5;

            go.AddComponent<Rigidbody2D>();
            go.AddComponent<CircleCollider2D>();
            go.AddComponent<EnemyController>();

            go.SetActive(false);
            go.transform.SetParent(this.transform);
            return go;
        }

        private GameObject CreatePowerUpPrefab()
        {
            var go = new GameObject("PowerUpPrefab");
            var sr = go.AddComponent<SpriteRenderer>();
            if (Art != null && Art.powerup != null) sr.sprite = Art.powerup;
            else sr.sprite = MakeDiamondSprite(new Color(1f, 0.95f, 0.4f), 64);
            sr.sortingOrder = 6;

            go.AddComponent<CircleCollider2D>();
            go.AddComponent<PowerUp>();

            go.SetActive(false);
            go.transform.SetParent(this.transform);
            return go;
        }

        private Bullet CreateBulletPrefab()
        {
            var go = new GameObject("BulletPrefab");
            var sr = go.AddComponent<SpriteRenderer>();
            if (Art != null && Art.bullet != null) sr.sprite = Art.bullet;
            else sr.sprite = MakeCircleSprite(new Color(1f, 1f, 0.8f), 32);
            sr.sortingOrder = 8;

            var b = go.AddComponent<Bullet>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<CircleCollider2D>();

            go.SetActive(false);
            go.transform.SetParent(this.transform);
            return b;
        }

        private HUDController CreateHUD()
        {
            var canvasGO = new GameObject("HUD");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            var mk = CreateText(canvasGO.transform, new Vector2(12, -12), "MK 1", 20);
            var score = CreateText(canvasGO.transform, new Vector2(12, -36), "0", 20);
            var lives = CreateText(canvasGO.transform, new Vector2(12, -60), "❤ 3", 20);

            var hpGO = new GameObject("HPBar");
            hpGO.transform.SetParent(canvasGO.transform, false);
            var rect = hpGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-12, -12);
            rect.sizeDelta = new Vector2(200, 18);

            var bg = new GameObject("BG");
            bg.transform.SetParent(hpGO.transform, false);
            var bgImg = bg.AddComponent<Image>();
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgImg.color = new Color(0, 0, 0, 0.4f);

            var slider = hpGO.AddComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            var fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(hpGO.transform, false);
            var faRect = fillArea.AddComponent<RectTransform>();
            faRect.anchorMin = new Vector2(0, 0);
            faRect.anchorMax = new Vector2(1, 1);
            faRect.offsetMin = new Vector2(2, 2);
            faRect.offsetMax = new Vector2(-2, -2);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(faRect, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(1f, 0.85f, 0.25f);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImg;
            slider.direction = Slider.Direction.LeftToRight;

            var hud = canvasGO.AddComponent<HUDController>();
            var go = new GameObject("GameOver");
            go.transform.SetParent(canvasGO.transform, false);
            var gorect = go.AddComponent<RectTransform>();
            gorect.anchorMin = new Vector2(0,0); gorect.anchorMax = new Vector2(1,1);
            gorect.offsetMin = gorect.offsetMax = Vector2.zero;
            var goimg = go.AddComponent<Image>(); goimg.color = new Color(0,0,0,0.6f);
            var gotxt = CreateText(go.transform, new Vector2(0, -20), "GAME OVER", 48);
            gotxt.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
            var trect = gotxt.GetComponent<RectTransform>();
            trect.anchorMin = trect.anchorMax = new Vector2(0.5f, 0.5f);
            trect.pivot = new Vector2(0.5f, 0.5f);
            trect.anchoredPosition = Vector2.zero;
            go.SetActive(false);

            hud.HpBar = slider;
            hud.ScoreText = score.GetComponent<UnityEngine.UI.Text>();
            hud.MKText = mk.GetComponent<UnityEngine.UI.Text>();
            hud.LivesText = lives.GetComponent<UnityEngine.UI.Text>();
            hud.GameOverPanel = go;
            return hud;
        }

        private GameObject CreateText(Transform parent, Vector2 anchored, string content, int size)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = anchored;
            var text = go.AddComponent<UnityEngine.UI.Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.color = new Color(1f, 0.95f, 0.6f);
            text.alignment = TextAnchor.UpperLeft;
            return go;
        }

        private void CreateArenaLines()
        {
            var cfg = ConfigOrDefault();
            float w = cfg.arenaSize.x;
            float h = cfg.arenaSize.y;

            CreateLine(new Vector2(-w/2, -h/2), new Vector2(w/2, -h/2));
            CreateLine(new Vector2(-w/2,  h/2), new Vector2(w/2,  h/2));
            CreateLine(new Vector2(-w/2, -h/2), new Vector2(-w/2,  h/2));
            CreateLine(new Vector2( w/2, -h/2), new Vector2( w/2,  h/2));
        }

        private void CreateLine(Vector2 a, Vector2 b)
        {
            var go = new GameObject("Line");
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
            lr.startWidth = lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = new Color(0.3f, 0.32f, 0.4f);
            lr.sortingOrder = 1;
        }

        // --- quick sprite makers (procedural placeholders) ---
        private Sprite MakeCircleSprite(Color color, int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var r = size * 0.5f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                var dx = x - r + 0.5f;
                var dy = y - r + 0.5f;
                var inside = (dx*dx + dy*dy) <= (r*r);
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(0.5f, 0.5f), 32f);
        }

        private Sprite MakeTriangleSprite(Color color, int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float fx = (x / (float)(size-1)) * 2f - 1f;
                float fy = (y / (float)(size-1)) * 2f - 1f;
                bool inside = fy >= -0.8f && fy <= 0.8f && Mathf.Abs(fx) <= (fy + 1f);
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(0.5f, 0.5f), 32f);
        }

        private Sprite MakeDiamondSprite(Color color, int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float r = size * 0.5f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Abs(x - r + 0.5f);
                float dy = Mathf.Abs(y - r + 0.5f);
                bool inside = dx + dy <= r * 0.9f;
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(0.5f, 0.5f), 32f);
        }
    }
}
