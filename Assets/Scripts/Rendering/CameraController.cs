using UnityEngine;

namespace TopDownShooter.Rendering
{
    public class CameraController : MonoBehaviour
    {
        public Transform Target;
        public float FollowLerp = 12f;
        public float OrthoSize = 12f;

        Camera _cam;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            if (_cam != null)
            {
                _cam.orthographic = true;
                _cam.orthographicSize = OrthoSize;
                _cam.clearFlags = CameraClearFlags.SolidColor;
                _cam.backgroundColor = Color.black;
            }
        }

        void LateUpdate()
        {
            if (Target == null) return;
            var p = transform.position;
            var t = Target.position;
            t.z = p.z;
            transform.position = Vector3.Lerp(p, t, 1f - Mathf.Exp(-FollowLerp * Time.deltaTime));
        }
    }
}
