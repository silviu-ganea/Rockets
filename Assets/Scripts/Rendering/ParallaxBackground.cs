using UnityEngine;

namespace TopDownShooter.Rendering
{
    public class ParallaxBackground : MonoBehaviour
    {
        public Transform Follow;
        public int Layers = 3;

        void Start()
        {
            for (int i = 0; i < Layers; i++)
            {
                var g = new GameObject("ParallaxLayer_" + i);
                g.transform.SetParent(transform, false);
                g.transform.position = new Vector3(0, 0, 5 + i); // behind gameplay

                var sr = g.AddComponent<SpriteRenderer>();
                sr.sprite = MakeStarfield(1024, 512, 120 * (i + 1), 0.25f + i * 0.25f);
                sr.sortingOrder = -100 - i;

                var layer = g.AddComponent<ParallaxLayer>();
                layer.Follow = Follow;
                layer.Multiplier = new Vector2(0.02f * (i + 1), 0.02f * (i + 1));
            }
        }

        Sprite MakeStarfield(int w, int h, int stars, float brightness)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var bg = new Color(0.05f, 0.06f, 0.1f, 1f);
            var pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;
            for (int i = 0; i < stars; i++)
            {
                int x = Random.Range(0, w), y = Random.Range(0, h);
                tex.SetPixel(x, y, Color.white * brightness);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f, 0.5f), 64f);
        }
    }
}
