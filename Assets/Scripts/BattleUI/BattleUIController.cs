using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AutoBattler
{
    public class BattleUIController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private RectTransform playerRow;
        [SerializeField] private RectTransform enemyRow;
        [SerializeField] private EmblemRollPanel emblemRollPanel;
        [SerializeField] private FloatingTextSpawner floatingTextSpawner;
        [SerializeField] private BattleResultPanel resultPanel;

        [Header("Prefabs")]
        [SerializeField] private PetView petViewPrefab;

        [Header("Colors")]
        [SerializeField] private Color damageColor = new Color(1f, 0.3f, 0.3f);

        private readonly Dictionary<Pet, PetView> _views = new();
        private readonly HashSet<Pet> _enemySide = new();
        
        private void OnEnable()
        {
            if (!battleManager)
                battleManager = FindAnyObjectByType<BattleManager>();
            if (!battleManager) return;

            // Subscribe to BattleManager events
            battleManager.OnPartyBuilt += HandlePartyBuilt;
            battleManager.OnDuelStart += HandleDuelStart;
            battleManager.OnRollResolved += HandleRollResolved;
            battleManager.OnPetDied += HandlePetDied;
            battleManager.OnBattleEnded += HandleBattleEnded;
        }

        private void OnDisable()
        {
            if (!battleManager) return;

            // Unsubscribe to prevent memory leaks
            battleManager.OnPartyBuilt -= HandlePartyBuilt;
            battleManager.OnDuelStart -= HandleDuelStart;
            battleManager.OnRollResolved -= HandleRollResolved;
            battleManager.OnPetDied -= HandlePetDied;
            battleManager.OnBattleEnded -= HandleBattleEnded;
        }

        // PARTY SETUP
        private void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            if (side == Side.Player)
                BuildRow(playerRow, pets, isEnemy: false);
            else
                BuildRow(enemyRow, pets, isEnemy: true);
        }

        private void BuildRow(RectTransform row, List<Pet> pets, bool isEnemy)
        {
            // Remove any previous UI children
            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

            // Instantiate UI PetViews
            foreach (var pet in pets)
            {
                var v = Instantiate(petViewPrefab, row);
                v.Setup(pet);
                _views[pet] = v;
                if (isEnemy) _enemySide.Add(pet);
            }
        }
        
        // BATTLE FLOW EVENTS
        private void HandleDuelStart(Pet p1, Pet p2)
        {
            HighlightOnly(p1, p2);
        }

        private void HighlightOnly(Pet a, Pet b)
        {
            foreach (var kv in _views)
                kv.Value.SetHighlight(false);

            if (_views.TryGetValue(a, out var av)) av.SetHighlight(true);
            if (_views.TryGetValue(b, out var bv)) bv.SetHighlight(true);
        }

        private void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            // Show emblem roll (both sides’ emblems and highlight winner)
            emblemRollPanel?.ShowSpin(p1.Emblems, e1, p2.Emblems, e2, winnerIndex);

            // Apply visual damage & text popups
            if (winnerIndex == 0)
            {
                // p1 wins: p2 takes damage
                if (_views.TryGetValue(p2, out var dv))
                {
                    dv.UpdateHp();
                    floatingTextSpawner?.Spawn(dv.GetComponent<RectTransform>(), "-1", damageColor);
                }
                if (_views.TryGetValue(p1, out var av))
                    av.PlayAttackNudge(toRight: true);
            }
            else if (winnerIndex == 1)
            {
                // p2 wins: p1 takes damage
                if (_views.TryGetValue(p1, out var dv))
                {
                    dv.UpdateHp();
                    floatingTextSpawner?.Spawn(dv.GetComponent<RectTransform>(), "-1", damageColor);
                }
                if (_views.TryGetValue(p2, out var av))
                    av.PlayAttackNudge(toRight: false);
            }
        }

        private void HandlePetDied(Pet pet)
        {
            if (!_views.TryGetValue(pet, out var v)) return;
            v.ShowDead();

            // Move to end of its row
            var row = v.transform.parent as RectTransform;
            v.transform.SetSiblingIndex(row.childCount - 1);
            v.SetHighlight(false);
        }

        private void HandleBattleEnded(Side winner)
        {
            Debug.Log($"[BattleUIController] HandleBattleEnded called. Winner: {winner}");
            if (winner == Side.Player)
                resultPanel.ShowVictory();
            else
                resultPanel.ShowDefeat();
        }
    }
}