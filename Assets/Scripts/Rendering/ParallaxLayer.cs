using UnityEngine;

namespace TopDownShooter.Rendering
{
    public class ParallaxLayer : MonoBehaviour
    {
        public Transform Follow;
        public Vector2 Multiplier = new Vector2(0.1f, 0.1f);

        Vector3 _startPos;

        void Start() => _startPos = transform.position;

        void LateUpdate()
        {
            if (Follow == null) return;
            var f = Follow.position;
            transform.position = _startPos + new Vector3(f.x * Multiplier.x, f.y * Multiplier.y, 0f);
        }
    }
}
