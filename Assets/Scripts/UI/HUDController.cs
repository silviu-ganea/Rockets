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
        public Text   LivesText;
        public GameObject GameOverPanel;

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
            player.OnMKChanged += (mk) => { if (MKText != null) MKText.text = $"MK {mk}"; };
        }

        public void SetScore(int s) { if (ScoreText) ScoreText.text = s.ToString("N0"); }
        public void SetLives(int l) { if (LivesText) LivesText.text = $"❤ {l}"; }

        public void ShowGameOver(int score)
        {
            if (GameOverPanel != null) GameOverPanel.SetActive(true);
            if (ScoreText) ScoreText.text = $"Score: {score:N0}";
        }
    }
}
