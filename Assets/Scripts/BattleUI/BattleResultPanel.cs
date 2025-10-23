using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AutoBattler
{
    public class BattleResultPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button tameButton;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Config")]
        [SerializeField] private int startingFood = 3;
        [SerializeField] private float messageDelay = 3f;
        [SerializeField] private float tameSuccessChance = 0.8f;

        private int _playerFood;
        private bool _battleOver = false;

        private void Awake()
        {
            _playerFood = startingFood;
            HidePanel();
        }

        public void ShowVictory()
        {
            _battleOver = true;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            resultText.text = "Victory!";
            tameButton.gameObject.SetActive(true);
            statusText.text = $"You have {_playerFood} food.";

            tameButton.onClick.RemoveAllListeners();
            tameButton.onClick.AddListener(AttemptTame);
        }

        public void ShowDefeat()
        {
            _battleOver = true;
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            resultText.text = "Defeat...";
            tameButton.gameObject.SetActive(false);
            statusText.text = "Returning to overworld...";
            StartCoroutine(ExitAfterDelay());
        }

        private void AttemptTame()
        {
            if (_playerFood <= 0)
            {
                tameButton.interactable = false;
                statusText.text = "You ran out of food :c";
                StartCoroutine(ExitAfterDelay());
                return;
            }

            _playerFood--;
            statusText.text = $"You used 1 food. {_playerFood} left...";

            bool success = Random.value < tameSuccessChance;
            if (success)
            {
                statusText.text = "You tamed the monster!";
                tameButton.interactable = false;
                StartCoroutine(ExitAfterDelay());
                // TODO: Add captured monster to player inventory later
            }
            else
            {
                statusText.text = "The monster resisted!";
                if (_playerFood <= 0)
                {
                    tameButton.interactable = false;
                    StartCoroutine(ExitAfterDelay());
                }
            }
        }

        private IEnumerator ExitAfterDelay()
        {
            yield return new WaitForSeconds(messageDelay);
            HidePanel();
            Debug.Log("Returning to overworld scene...");
            // TODO: SceneManager.LoadScene("OverworldScene");
        }

        private void HidePanel()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}