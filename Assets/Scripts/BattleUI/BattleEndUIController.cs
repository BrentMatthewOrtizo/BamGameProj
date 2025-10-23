using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using AutoBattler;
using Game.Runtime;
using Game.com.game399.shared.Services.Implementation;
using Game.com.game399.shared.Models;

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
    [SerializeField] private float exitDelaySeconds = 2f;

    [Header("Monster Sprites")]
    [SerializeField] private Sprite evilFox;
    [SerializeField] private Sprite evilCamel;
    [SerializeField] private Sprite evilChimera;
    [SerializeField] private Sprite tamedFox;
    [SerializeField] private Sprite tamedCamel;
    [SerializeField] private Sprite tamedChimera;

    private BattleManager _battleManager;
    private InventoryViewModel _vm;
    private Pet _tameTarget;
    private string _tameTargetName;

    void Start()
    {
        SetUIVisible(false);

        _vm = ServiceResolver.Resolve<InventoryViewModel>();
        _battleManager = FindAnyObjectByType<BattleManager>();

        if (_battleManager != null)
            _battleManager.OnBattleEnded += HandleBattleEnd;
        else
            Debug.LogWarning("BattleEndUIController: No BattleManager found in scene.");

        tameButton.onClick.AddListener(HandleTameButton);
    }

    void OnDestroy()
    {
        if (_battleManager != null)
            _battleManager.OnBattleEnded -= HandleBattleEnd;
        tameButton.onClick.RemoveListener(HandleTameButton);
    }

    // -----------------------------------------------------------
    //  UI Management
    // -----------------------------------------------------------
    private void SetUIVisible(bool visible)
    {
        overlay.alpha = visible ? 1 : 0;
        overlay.blocksRaycasts = visible;
        overlay.interactable = visible;
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
        overlay.alpha = to;
    }

    // -----------------------------------------------------------
    //  Battle End Handling
    // -----------------------------------------------------------
    private void HandleBattleEnd(Side winner)
    {
        StartCoroutine(ShowBattleEndSequence(winner));
    }

    private IEnumerator ShowBattleEndSequence(Side winner)
    {
        SetUIVisible(true);
        monsterImage.gameObject.SetActive(false);
        tameButton.gameObject.SetActive(false);

        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeInDuration));

        var playerParty = ServiceResolver.Resolve<PlayerPartyModel>();

        if (winner == Side.Player)
        {
            messageText.text = "Victory!!!";
            yield return new WaitForSeconds(messageDisplayTime);

            // If no food
            if (_vm.FoodCount.Value <= 0)
            {
                yield return StartCoroutine(OutOfFoodThenExit());
                yield break;
            }

            // If party is full
            if (playerParty.IsFull)
            {
                messageText.text = "Uh oh, you have no pet space :(";
                yield return new WaitForSeconds(exitDelaySeconds);
                SetUIVisible(false);
                ExitBackToPreviousScene();
                yield break;
            }

            // Begin taming
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
            RefreshTameButton();
        }
        else
        {
            messageText.text = "Defeat...";
            yield return new WaitForSeconds(exitDelaySeconds);
            SetUIVisible(false);
            ExitBackToPreviousScene();
        }
    }

    // -----------------------------------------------------------
    //  Tame Logic
    // -----------------------------------------------------------
    private void HandleTameButton()
    {
        if (_vm.FoodCount.Value <= 0)
        {
            StartCoroutine(OutOfFoodThenExit());
            return;
        }

        _vm.AddFood(-1);
        RefreshTameButton();

        bool success = Random.value <= 0.8f;
        StartCoroutine(HandleTameOutcome(success));
    }

    private IEnumerator HandleTameOutcome(bool success)
    {
        var playerParty = ServiceResolver.Resolve<PlayerPartyModel>();

        if (success)
        {
            messageText.text = "You Tamed It!";
            yield return new WaitForSeconds(0.3f);

            // Flip to tamed sprite
            monsterImage.sprite = GetTamedSpriteFor(_tameTargetName);
            monsterImage.color = Color.white;

            // Add to persistent party
            playerParty.AddMonster(new TamedMonster(_tameTarget.Name, _tameTarget.MaxHP, _tameTarget.Damage));

            yield return new WaitForSeconds(exitDelaySeconds);
            SetUIVisible(false);
            ExitBackToPreviousScene();
        }
        else
        {
            if (_vm.FoodCount.Value > 0)
            {
                messageText.text = "It resisted… try again?";
                tameButton.interactable = true;
                RefreshTameButton();
            }
            else
            {
                yield return StartCoroutine(OutOfFoodThenExit());
            }
        }
    }

    private IEnumerator OutOfFoodThenExit()
    {
        tameButton.gameObject.SetActive(false);
        monsterImage.gameObject.SetActive(false);
        messageText.text = "You ran out of food...";
        yield return new WaitForSeconds(exitDelaySeconds);
        SetUIVisible(false);
        ExitBackToPreviousScene();
    }

    private void RefreshTameButton()
    {
        var tmp = tameButton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp)
            tmp.text = $"Tame? (Cost 1 Food)  •  You have {_vm.FoodCount.Value}";
        tameButton.interactable = _vm.FoodCount.Value > 0;
    }

    // -----------------------------------------------------------
    //  Utilities
    // -----------------------------------------------------------
    private List<Pet> GetEnemyPets()
    {
        var bm = FindAnyObjectByType<BattleManager>();
        if (bm == null) return new List<Pet>();
        var field = typeof(BattleManager).GetField("_enemy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(bm) as List<Pet> ?? new List<Pet>();
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

    private void ExitBackToPreviousScene()
    {
        var target = string.IsNullOrEmpty(WarpManager.previousSceneName)
            ? "Game"
            : WarpManager.previousSceneName;
        SceneManager.LoadScene(target);
    }
}