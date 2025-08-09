using UnityEngine;

namespace TopDownShooter.Core
{
    public static class Art
    {
        public static Sprite PickEnemy(ArtConfig art)
        {
            if (art == null || art.enemyShips == null || art.enemyShips.Length == 0) return null;
            var i = Random.Range(0, art.enemyShips.Length);
            return art.enemyShips[i];
        }
    }
}
