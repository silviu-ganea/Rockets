using UnityEngine;

namespace TopDownShooter.Core
{
    [CreateAssetMenu(menuName = "TopDownShooter/ArtConfig")]
    public class ArtConfig : ScriptableObject
    {
        [Header("Gameplay Sprites")]
        public Sprite playerShip;
        public Sprite bullet;
        public Sprite powerup;

        [Tooltip("One or more enemy ship sprites; a random one is chosen per spawn.")]
        public Sprite[] enemyShips;

        [Header("Optional")]
        public Sprite backgroundSprite;
    }
}
