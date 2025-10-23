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

        private void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            var row    = (side == Side.Player) ? playerRow : enemyRow;
            bool enemy = (side == Side.Enemy);

            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

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

        private void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            if (winnerIndex == 0) // player wins → enemy took damage
            {
                if (_views.TryGetValue(p2, out var t)) { t.UpdateHp(); t.FlashHit(); }
            }
            else if (winnerIndex == 1) // enemy wins → player took damage
            {
                if (_views.TryGetValue(p1, out var t)) { t.UpdateHp(); t.FlashHit(); }
            }
        }

        private void HandlePetDied(Pet pet)
        {
            if (_views.TryGetValue(pet, out var v))
                v.ShowDeadVisual();
            // Row gets rebuilt by BattleManager.HandleDeath → HandlePartyBuilt runs again.
        }
    }
}