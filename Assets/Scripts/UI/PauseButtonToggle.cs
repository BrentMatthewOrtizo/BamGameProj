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
            button.onClick.AddListener(TogglePause);
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            battleManager.SetPaused(isPaused);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = isPaused ? "Resume" : "Pause";
            Debug.Log(isPaused ? "Game Paused." : "Game Resumed.");
        }
    }
}