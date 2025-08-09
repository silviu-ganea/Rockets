using UnityEngine;

namespace TopDownShooter
{
    public static class AutoBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (Object.FindObjectOfType<Bootstrapper>() != null) return;
            var go = new GameObject("Bootstrap (auto)");
            go.hideFlags = HideFlags.DontSave;
            go.AddComponent<Bootstrapper>();
        }
    }
}
