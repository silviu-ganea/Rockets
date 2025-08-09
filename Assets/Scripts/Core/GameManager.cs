using TopDownShooter.Entities;
using TopDownShooter.UI;
using UnityEngine;

namespace TopDownShooter.Core
{
    public class GameManager : MonoBehaviour
    {
        public int Lives = 3;
        public int Score = 0;

        HUDController _hud;
        PlayerController _player;

        public void Bind(HUDController hud, PlayerController player)
        {
            _hud = hud; _player = player;
            _hud.SetLives(Lives);
            _hud.SetScore(Score);
            player.OnHPChanged += (hp, max) => { if (hp <= 0) OnPlayerDown(); };
        }

        public void AddScore(int v = 1)
        {
            Score += v;
            _hud?.SetScore(Score);
        }

        void OnPlayerDown()
        {
            Lives = Mathf.Max(0, Lives - 1);
            _hud?.SetLives(Lives);
            if (Lives <= 0)
            {
                Time.timeScale = 0f;
                _hud?.ShowGameOver(Score);
            }
            else
            {
                // simple respawn: heal and continue
                _player.Heal(_player.MaxHP);
            }
        }
    }
}
