using UnityEngine;

namespace TopDownShooter.Rendering
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public Transform Target;
        public float FollowLerp = 12f;
        public float OrthoSize = 36f; // default wider
        Camera _cam;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            _cam.orthographic = true;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = Color.black;
        }

        void LateUpdate()
        {
            // keep size in sync with the public field
            if (_cam != null) _cam.orthographicSize = OrthoSize;

            if (Target == null) return;
            var p = transform.position;
            var t = Target.position; t.z = p.z;
            transform.position = Vector3.Lerp(p, t, 1f - Mathf.Exp(-FollowLerp * Time.deltaTime));
        }
    }
}
