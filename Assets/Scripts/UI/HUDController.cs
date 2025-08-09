using TopDownShooter.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter.UI
{
    public class HUDController : MonoBehaviour
    {
        public Slider HpBar;
        public Text   ScoreText;
        public Text   MKText;

        private int _score;

        public void Bind(PlayerController player)
        {
            player.OnHPChanged += (hp, max) =>
            {
                if (HpBar != null)
                {
                    HpBar.maxValue = max;
                    HpBar.value = hp;
                }
            };
            player.OnMKChanged += (mk) =>
            {
                if (MKText != null) MKText.text = $"MK {mk}";
            };
        }

        public void AddScore(int v = 1)
        {
            _score += v;
            if (ScoreText != null) ScoreText.text = _score.ToString("N0");
        }
    }
}
