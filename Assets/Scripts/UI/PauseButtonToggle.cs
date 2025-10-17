using UnityEngine;
using UnityEngine.UI;

namespace AutoBattler
{
    public class PauseButtonToggle : MonoBehaviour
    {
        public BattleManager battleManager;
        private Button button;
        private bool isPaused = false;

        void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("PauseButtonToggle: No Button component found!");
                return;
            }

            if (battleManager == null)
            {
                Debug.LogError("PauseButtonToggle: BattleManager reference not assigned!");
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
                    Debug.LogWarning("PauseButtonToggle: No Text component found on the PauseButton.");
                }
            }

            Debug.Log(isPaused ? "Game Paused." : "Game Resumed.");
        }
    }
}