using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private void OnEnable()
        {
            if (!battleManager)
                battleManager = FindAnyObjectByType<BattleManager>();

            if (!battleManager)
            {
                Debug.LogWarning("[BattleUIController] BattleManager not found!");
                return;
            }

            battleManager.OnPartyBuilt += HandlePartyBuilt;
            battleManager.OnRollResolved += HandleRollResolved;
            battleManager.OnPetDied += HandlePetDied;
        }

        private void OnDisable()
        {
            if (!battleManager) return;

            battleManager.OnPartyBuilt -= HandlePartyBuilt;
            battleManager.OnRollResolved -= HandleRollResolved;
            battleManager.OnPetDied -= HandlePetDied;
        }

        private void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            RectTransform row = side == Side.Player ? playerRow : enemyRow;
            bool isEnemy = side == Side.Enemy;

            // clear existing UI
            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

            // build new PetViews
            foreach (var pet in pets)
            {
                var view = Instantiate(petViewPrefab, row);
                view.Setup(pet, battleManager.damagePerWin, null, isEnemy);
                _views[pet] = view;

                if (isEnemy)
                    _enemySide.Add(pet);
            }
        }

        private void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            if (winnerIndex == 0)
            {
                // Player wins
                if (_views.TryGetValue(p2, out var target))
                {
                    target.UpdateHp();
                    target.FlashHit();
                }

                if (_views.TryGetValue(p1, out var attacker))
                    attacker.PlayAttackNudge(toRight: true);
            }
            else if (winnerIndex == 1)
            {
                // Enemy wins
                if (_views.TryGetValue(p1, out var target))
                {
                    target.UpdateHp();
                    target.FlashHit();
                }

                if (_views.TryGetValue(p2, out var attacker))
                    attacker.PlayAttackNudge(toRight: false);
            }
        }

        private void HandlePetDied(Pet pet)
        {
            if (_views.TryGetValue(pet, out var view))
                view.ShowDead();
        }
    }
}