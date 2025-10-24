using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AutoBattler
{
    public class BattleUIController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private RectTransform playerRow;
        [SerializeField] private RectTransform enemyRow;
        [SerializeField] private TextMeshProUGUI battlePopupText;

        [Header("Prefabs")]
        [SerializeField] private PetView petViewPrefab;

        // 👇 NEW: tamed (player) sprites
        [Header("Player (Tamed) Sprites")]
        [SerializeField] private Sprite tamedFox;
        [SerializeField] private Sprite tamedCamel;
        [SerializeField] private Sprite tamedChimera;

        private readonly Dictionary<Pet, PetView> _views = new();
        private readonly HashSet<Pet> _enemySide = new();

        void OnEnable()
        {
            if (!battleManager) battleManager = FindAnyObjectByType<BattleManager>();
            if (!battleManager) return;

            battleManager.OnBattleStart       += HandleBattleStart;
            battleManager.OnPartyBuilt        += HandlePartyBuilt;
            battleManager.OnRollStart         += HandleRollStart;
            battleManager.OnRollResolved      += HandleRollResolved;
            battleManager.OnDamageApplied     += HandleDamageApplied;
            battleManager.OnPetDied           += HandlePetDied;
            battleManager.OnPetKilledFinal    += HandlePetKilledFinal;
        }

        void OnDisable()
        {
            if (!battleManager) return;

            battleManager.OnBattleStart       -= HandleBattleStart;
            battleManager.OnPartyBuilt        -= HandlePartyBuilt;
            battleManager.OnRollStart         -= HandleRollStart;
            battleManager.OnRollResolved      -= HandleRollResolved;
            battleManager.OnDamageApplied     -= HandleDamageApplied;
            battleManager.OnPetDied           -= HandlePetDied;
            battleManager.OnPetKilledFinal    -= HandlePetKilledFinal;
        }

        private void HandleBattleStart() => StartCoroutine(ShowBattlePopup());

        private IEnumerator ShowBattlePopup()
        {
            if (!battlePopupText) yield break;

            battlePopupText.gameObject.SetActive(true);
            battlePopupText.alpha = 1f;
            battlePopupText.text = "Battle Encountered!";
            battlePopupText.transform.localScale = Vector3.one * 0.5f;

            Vector3 targetScale = Vector3.one * 1.2f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                battlePopupText.transform.localScale = Vector3.Lerp(battlePopupText.transform.localScale, targetScale, t);
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                battlePopupText.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            battlePopupText.alpha = 1f;
            battlePopupText.gameObject.SetActive(false);
        }

        private void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            var row = (side == Side.Player) ? playerRow : enemyRow;
            bool enemy = (side == Side.Enemy);

            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

            foreach (var pet in pets)
                CreateView(row, pet, isEnemy: enemy);
        }

        private void CreateView(RectTransform row, Pet pet, bool isEnemy)
        {
            // 👇 Choose a sprite only for player; enemies handled by PetView internally
            Sprite spriteToUse = null;
            if (!isEnemy)
                spriteToUse = GetPlayerSpriteFor(pet.Name);

            var v = Instantiate(petViewPrefab, row);
            v.Setup(pet, battleManager.damagePerWin, spriteToUse, isEnemy);
            _views[pet] = v;
            if (isEnemy) _enemySide.Add(pet);
        }

        // 👇 NEW: simple name->tamed sprite mapping
        private Sprite GetPlayerSpriteFor(string petName)
        {
            var n = (petName ?? "").ToLowerInvariant();
            if (n.Contains("camel"))   return tamedCamel;
            if (n.Contains("chimera")) return tamedChimera;
            if (n.Contains("fox"))     return tamedFox;
            return tamedFox; // fallback
        }

        private void HandleRollStart(Pet p1, Pet p2)
        {
            foreach (var v in _views.Values)
                v.HideEmblem();

            if (_views.TryGetValue(p1, out var v1))
                v1.BeginEmblemRoll(battleManager.rollCycleInterval);

            if (_views.TryGetValue(p2, out var v2))
                v2.BeginEmblemRoll(battleManager.rollCycleInterval);
        }

        private void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            if (_views.TryGetValue(p1, out var v1))
                v1.EndEmblemRoll(e1);

            if (_views.TryGetValue(p2, out var v2))
                v2.EndEmblemRoll(e2);
        }

        private void HandleDamageApplied(Pet p1, Pet p2, int winnerIndex)
        {
            if (winnerIndex == 0)
            {
                if (_views.TryGetValue(p2, out var t)) { t.UpdateHp(); t.FlashHit(); }
            }
            else if (winnerIndex == 1)
            {
                if (_views.TryGetValue(p1, out var t)) { t.UpdateHp(); t.FlashHit(); }
            }
        }

        private void HandlePetDied(Pet pet)
        {
            if (_views.TryGetValue(pet, out var v))
                v.ShowDeadVisual();
        }

        private void HandlePetKilledFinal(Pet pet, Side side)
        {
            if (_views.TryGetValue(pet, out var v))
            {
                v.PlayDeathHit();
                v.ShowDeadVisual();
            }
        }
    }
}