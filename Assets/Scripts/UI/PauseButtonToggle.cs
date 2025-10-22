using UnityEngine;
using UnityEngine.UI;
using Game399.Shared.Diagnostics;
using Game.Runtime;

namespace AutoBattler
{
    public class PauseButtonToggle : MonoBehaviour
    {
        
        private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
        public BattleManager battleManager;
        private Button button;
        private bool isPaused = false;

        void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Log.Error("PauseButtonToggle: No Button component found!");
                return;
            }

            if (battleManager == null)
            {
                Log.Error("PauseButtonToggle: BattleManager reference not assigned!");
                return;
            }

            button.onClick.AddListener(TogglePause);
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            battleManager.SetPaused(isPaused);

            // Try to find TextMeshPro first
            var tmp = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = isPaused ? "Resume" : "Pause";
            }
            else
            {
                // Fallback to Legacy Text
                var legacy = button.GetComponentInChildren<UnityEngine.UI.Text>();
                if (legacy != null)
                {
                    legacy.text = isPaused ? "Resume" : "Pause";
                }
                else
                {
                    Log.Warn("PauseButtonToggle: No Text component found on the PauseButton.");
                }
            }

            Log.Info(isPaused ? "Game Paused." : "Game Resumed.");
        }
    }
}