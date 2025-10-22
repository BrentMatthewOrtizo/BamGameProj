using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattler
{
    public class BattleUIController : MonoBehaviour
    {
        [Header("Scene Refs")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private RectTransform playerRow;
        [SerializeField] private RectTransform enemyRow;
        [SerializeField] private EmblemRollPanel emblemRollPanel;
        [SerializeField] private FloatingTextSpawner floatingTextSpawner;

        [Header("Prefabs")]
        [SerializeField] private PetView petViewPrefab;

        [Header("Colors")]
        [SerializeField] private Color damageColor = new Color(1f, .3f, .3f);

        readonly Dictionary<Pet, PetView> _views = new();
        readonly HashSet<Pet> _enemySide = new();

        void OnEnable()
        {
            if (!battleManager) battleManager = FindFirstObjectByType<BattleManager>();
            if (!battleManager) return;

            battleManager.OnPartyBuilt += HandlePartyBuilt;
            battleManager.OnDuelStart += HandleDuelStart;
            battleManager.OnRollResolved += HandleRollResolved;
            battleManager.OnPetDied += HandlePetDied;
            // battleManager.OnBattleEnded -> we’ll hook for ResultPanel later
        }

        void OnDisable()
        {
            if (!battleManager) return;
            battleManager.OnPartyBuilt -= HandlePartyBuilt;
            battleManager.OnDuelStart -= HandleDuelStart;
            battleManager.OnRollResolved -= HandleRollResolved;
            battleManager.OnPetDied -= HandlePetDied;
        }

        void HandlePartyBuilt(Side side, List<Pet> pets)
        {
            if (side == Side.Player)
                BuildRow(playerRow, pets, isEnemy:false);
            else
                BuildRow(enemyRow, pets, isEnemy:true);
        }

        void BuildRow(RectTransform row, List<Pet> pets, bool isEnemy)
        {
            // clear old
            for (int i = row.childCount - 1; i >= 0; i--) Destroy(row.GetChild(i).gameObject);

            foreach (var pet in pets)
            {
                var v = Instantiate(petViewPrefab, row);
                v.Setup(pet);
                _views[pet] = v;
                if (isEnemy) _enemySide.Add(pet);
            }
        }

        void HandleDuelStart(Pet p1, Pet p2)
        {
            // highlight front pets
            HighlightOnly(p1, p2);
        }

        void HighlightOnly(Pet a, Pet b)
        {
            foreach (var kv in _views)
                kv.Value.SetHighlight(false);

            if (_views.TryGetValue(a, out var av)) av.SetHighlight(true);
            if (_views.TryGetValue(b, out var bv)) bv.SetHighlight(true);
        }

        void HandleRollResolved(Pet p1, Pet p2, Emblem e1, Emblem e2, int winnerIndex)
        {
            // spin panel
            emblemRollPanel?.ShowSpin(p1.Emblems, e1, p2.Emblems, e2, winnerIndex);

            // show damage number & attack nudge
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
            // tie => nothing
        }

        void HandlePetDied(Pet pet)
        {
            if (!_views.TryGetValue(pet, out var v)) return;
            v.ShowDead();

            // move to end of its row
            var row = v.transform.parent as RectTransform;
            v.transform.SetSiblingIndex(row.childCount - 1);
            v.SetHighlight(false);
        }
    }
}