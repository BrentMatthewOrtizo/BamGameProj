using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using AutoBattler;

public class BattleEndUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image monsterImage;
    [SerializeField] private Button tameButton;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 0.6f;
    [SerializeField] private float messageDisplayTime = 1.5f;

    [Header("Monster Sprites")]
    [SerializeField] private Sprite evilFox;
    [SerializeField] private Sprite evilCamel;
    [SerializeField] private Sprite evilChimera;
    [SerializeField] private Sprite tamedFox;
    [SerializeField] private Sprite tamedCamel;
    [SerializeField] private Sprite tamedChimera;

    private BattleManager _battleManager;
    private Pet _tameTarget;
    private string _tameTargetName;

    private void Start()
    {
        SetUIVisible(false);

        _battleManager = FindAnyObjectByType<BattleManager>();
        if (_battleManager != null)
            _battleManager.OnBattleEnded += HandleBattleEnd;
        else
            Debug.LogWarning("BattleEndUIController: No BattleManager found in scene.");

        tameButton.onClick.AddListener(HandleTameButton);
    }

    private void OnDestroy()
    {
        if (_battleManager != null)
            _battleManager.OnBattleEnded -= HandleBattleEnd;
    }

    private void SetUIVisible(bool visible)
    {
        overlay.alpha = visible ? 1 : 0;
        overlay.blocksRaycasts = visible;
        overlay.interactable = visible;
    }

    // ---------------------------------------------------------------------
    // Called automatically when battle ends
    // ---------------------------------------------------------------------
    private void HandleBattleEnd(Side winner)
    {
        StartCoroutine(ShowBattleEndSequence(winner));
    }

    private IEnumerator ShowBattleEndSequence(Side winner)
    {
        SetUIVisible(true);
        monsterImage.gameObject.SetActive(false);
        tameButton.gameObject.SetActive(false);

        // Fade in overlay
        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeInDuration));

        if (winner == Side.Player)
        {
            // Step 1: Show victory
            messageText.text = "Victory!!!";
            yield return new WaitForSeconds(messageDisplayTime);

            // Step 2: Switch to Tame Time
            messageText.text = "Woah! Tame Time!";
            yield return new WaitForSeconds(0.5f);

            var enemyPets = GetEnemyPets();
            if (enemyPets.Count > 0)
            {
                _tameTarget = enemyPets[Random.Range(0, enemyPets.Count)];
                _tameTargetName = _tameTarget.Name.ToLower();
                monsterImage.sprite = GetEvilSpriteFor(_tameTargetName);
                monsterImage.color = Color.white;
                monsterImage.gameObject.SetActive(true);
            }

            tameButton.gameObject.SetActive(true);
            tameButton.interactable = true;
            tameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tame? (Cost 1 Food)";
        }
        else
        {
            // Player lost the battle
            messageText.text = "Defeat...";
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(WarpManager.previousSceneName);
        }
    }

    // ---------------------------------------------------------------------
    // Handle Tame Attempt
    // ---------------------------------------------------------------------
    private void HandleTameButton()
    {
        tameButton.interactable = false;

        // TODO: integrate food check later
        bool success = Random.value <= 0.8f;

        StartCoroutine(HandleTameOutcome(success));
    }

    private IEnumerator HandleTameOutcome(bool success)
    {
        if (success)
        {
            messageText.text = "You Tamed It!";
            yield return new WaitForSeconds(0.3f);

            monsterImage.sprite = GetTamedSpriteFor(_tameTargetName);
            monsterImage.color = Color.white;

            yield return new WaitForSeconds(2f);

            // Later TODO: add to inventory here
            // InventoryManager.Instance.AddMonster(new MonsterClass(_tameTargetName, ...));
        }
        else
        {
            messageText.text = "You ran out of food...";
            yield return new WaitForSeconds(2f);
        }

        // Return to overworld scene where player last was
        SceneManager.LoadScene(WarpManager.previousSceneName);
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------
    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            overlay.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
    }

    private List<Pet> GetEnemyPets()
    {
        var battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null) return new List<Pet>();

        var enemyField = typeof(BattleManager).GetField("_enemy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        return enemyField?.GetValue(battleManager) as List<Pet> ?? new List<Pet>();
    }

    private Sprite GetEvilSpriteFor(string name)
    {
        switch (name)
        {
            case "fox": return evilFox;
            case "camel": return evilCamel;
            case "chimera": return evilChimera;
            default: return null;
        }
    }

    private Sprite GetTamedSpriteFor(string name)
    {
        switch (name)
        {
            case "fox": return tamedFox;
            case "camel": return tamedCamel;
            case "chimera": return tamedChimera;
            default: return null;
        }
    }
}