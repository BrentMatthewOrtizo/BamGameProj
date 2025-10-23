using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattler
{
    public class BattleUIController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private RectTransform playerRow;
        [SerializeField] private RectTransform enemyRow;

        [Header("Prefabs")]
        [SerializeField] private PetView petViewPrefab;

        private readonly Dictionary<Pet, PetView> _views = new();
        private readonly HashSet<Pet> _enemySide = new();

        void OnEnable()
        {
            if (!battleManager) battleManager = FindAnyObjectByType<BattleManager>();
            if (!battleManager) return;

            battleManager.OnPartyBuilt += HandlePartyBuilt;
            battleManager.OnRollResolved += HandleRollResolved;
            battleManager.OnPetDied += HandlePetDied;
        }

        void OnDisable()
        {
            if (!battleManager) return;
            battleManager.OnPartyBuilt -= HandlePartyBuilt;
            battleManager.OnRollResolved -= HandleRollResolved;
            battleManager.OnPetDied -= HandlePetDied;
        }

        // ---------------------------------------------------------
        // BUILD PARTY ROWS
        // ---------------------------------------------------------
        private void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            var row = (side == Side.Player) ? playerRow : enemyRow;
            bool enemy = (side == Side.Enemy);

            // Clear old views
            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

            // Rebuild row in logical display order
            foreach (var pet in pets)
                CreateView(row, pet, isEnemy: enemy);
        }

        private void CreateView(RectTransform row, Pet pet, bool isEnemy)
        {
            var v = Instantiate(petViewPrefab, row);
            v.Setup(pet, battleManager.damagePerWin, null, isEnemy);
            _views[pet] = v;
            if (isEnemy) _enemySide.Add(pet);
        }

        // ---------------------------------------------------------
        // HANDLE ROLL RESOLUTION
        // ---------------------------------------------------------
        private void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            // Hide all emblems initially
            foreach (var view in _views.Values)
                view.HideEmblem();

            // Only active pets show rolling icons
            if (_views.TryGetValue(p1, out var v1))
            {
                v1.PlayEmblemRoll();
                StartCoroutine(ShowFinalEmblem(v1, e1));
            }

            if (_views.TryGetValue(p2, out var v2))
            {
                v2.PlayEmblemRoll();
                StartCoroutine(ShowFinalEmblem(v2, e2));
            }

            // Handle the actual damage + flash after emblem reveal
            StartCoroutine(HandlePostRoll(p1, p2, winnerIndex));
        }

        private IEnumerator ShowFinalEmblem(PetView view, Emblem emblem)
        {
            // Wait for roll animation to finish
            yield return new WaitForSeconds(0.6f);
            view.ShowEmblem(emblem);
        }

        private IEnumerator HandlePostRoll(Pet p1, Pet p2, int winnerIndex)
        {
            // Wait until emblems are revealed
            yield return new WaitForSeconds(0.8f);

            if (winnerIndex == 0) // player wins → enemy takes damage
            {
                if (_views.TryGetValue(p2, out var target))
                {
                    target.UpdateHp();
                    target.FlashHit();
                    if (!p2.IsAlive) target.ShowDeadVisual();
                }
            }
            else if (winnerIndex == 1) // enemy wins → player takes damage
            {
                if (_views.TryGetValue(p1, out var target))
                {
                    target.UpdateHp();
                    target.FlashHit();
                    if (!p1.IsAlive) target.ShowDeadVisual();
                }
            }
        }

        // ---------------------------------------------------------
        // HANDLE DEATH EVENT
        // ---------------------------------------------------------
        private void HandlePetDied(Pet pet)
        {
            if (_views.TryGetValue(pet, out var v))
                v.ShowDeadVisual();
            // BattleManager triggers row rebuild automatically afterwards.
        }
    }
}