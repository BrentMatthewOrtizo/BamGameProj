using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private BattleManager _battleManager;
    private Pet _tameTarget;

    private void Start()
    {
        // Hide all UI initially
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

    // Called when the battle ends
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

            // Show a random enemy monster sprite (placeholder)
            var enemyPets = GetEnemyPets();
            if (enemyPets.Count > 0)
            {
                _tameTarget = enemyPets[Random.Range(0, enemyPets.Count)];
                monsterImage.sprite = GetEvilSpriteFor(_tameTarget.Name);
                monsterImage.gameObject.SetActive(true);
            }

            // Step 3: Enable tame button
            tameButton.gameObject.SetActive(true);
            tameButton.interactable = true;
            tameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tame? (Cost 1 Food)";
        }
        else
        {
            // Defeat case
            messageText.text = "Defeat...";
            yield return new WaitForSeconds(2f);

            // Exit logic (placeholder for now)
            Debug.Log("Returning to overworld...");
            SetUIVisible(false);
        }
    }

    // Handle tame button press
    private void HandleTameButton()
    {
        // Random success (80%)
        bool success = Random.value <= 0.8f;
        tameButton.interactable = false;
        StartCoroutine(HandleTameOutcome(success));
    }

    private IEnumerator HandleTameOutcome(bool success)
    {
        if (success)
        {
            messageText.text = "You Tamed It!";
            monsterImage.color = Color.white; // show bright version
            yield return new WaitForSeconds(2f);
        }
        else
        {
            messageText.text = "You ran out of food...";
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("Returning to overworld scene...");
        SetUIVisible(false);
    }

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

    // Placeholder until we connect to enemy data
    private List<Pet> GetEnemyPets()
    {
        var battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null) return new List<Pet>();
        var enemyField = typeof(BattleManager).GetField("_enemy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return enemyField?.GetValue(battleManager) as List<Pet> ?? new List<Pet>();
    }

    private Sprite GetEvilSpriteFor(string name)
    {
        // Simple lookup for now — replace later with centralized sprite manager
        name = name.ToLower();
        return Resources.Load<Sprite>($"Art/MonsterSprites/evil-{name}") ?? null;
    }
}