using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace AutoBattler
{
    
    public enum Side { Player, Enemy }

    /// <summary>
    /// Top-level battle driver: builds parties, runs the duel loop, fires events.
    /// Attach this to an empty GameObject in the scene (e.g., "BattleManager").
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Maximum units in each party (prototype uses 3).")]
        [Range(1, 5)] public int maxPartySize = 3;

        [Tooltip("Damage dealt when an emblem wins a roll.")]
        public int damagePerWin = 1;

        [Tooltip("Delay between emblem rolls (for UI timing).")]
        [Range(0f, 1f)] public float rollDelaySeconds = 0.35f;

        [Tooltip("Seed for reproducible tests. Leave 0 for random.")]
        public int rngSeed = 0;

        [Tooltip("Automatically start battle on Start()")]
        public bool autoStart = true;

        [Header("Player Party (Inspector)")]
        public List<PetConfig> playerPartyConfig = new()
        {
            new PetConfig("Fox", 5,  Emblem.Sword, Emblem.Sword),
            new PetConfig("Camel", 6, Emblem.Shield),
            new PetConfig("Chimera", 7, Emblem.Magic, Emblem.Magic),
        };

        [Header("Enemy Party (Generated)")]
        [Tooltip("Enemy HP range inclusive (min..max).")]
        public Vector2Int enemyHpRange = new(5, 7);

        [Tooltip("How many emblems per enemy pet (1..3).")]
        public Vector2Int enemyEmblemCount = new(1, 3);

        // Runtime state
        private readonly List<Pet> _player = new();
        private readonly List<Pet> _enemy  = new();
        private Random _rng;

        #region Events (hook UI/FX here)
        public event Action OnBattleStart;
        public event Action<Side, List<Pet>> OnPartyBuilt;
        public event Action<Pet, Pet> OnDuelStart;                         // pets have met in middle
        public event Action<Pet, Pet, Emblem, Emblem, int> OnRollResolved; // winnerIndex: -1=tie, 0=left(p1), 1=right(p2)
        public event Action<Pet> OnPetDied;
        public event Action<Side /*winner*/> OnBattleEnded;
        #endregion
        
        private void OnEnable()
        {
            OnBattleStart += () => Debug.Log("Battle started!");
            OnPartyBuilt += (side, pets) => Debug.Log($"{side} party built with {pets.Count} pets.");
            OnDuelStart += (p1, p2) => Debug.Log($"Duel Start: {p1.Name} vs {p2.Name}");
            OnRollResolved += (p1, p2, e1, e2, result) => Debug.Log($"Roll: {p1.Name} [{e1}] vs {p2.Name} [{e2}] → {(result == 0 ? "P1 Wins" : result == 1 ? "P2 Wins" : "Tie")}");
            OnPetDied += p => Debug.Log($"{p.Name} has fallen!");
            OnBattleEnded += winner => Debug.Log($"Battle ended — Winner: {winner}");
        }

        #region Inspector helper types
        [Serializable]
        public class PetConfig
        {
            public string name = "Pet";
            public int maxHp = 6;
            [EmblemArray(3)]
            public Emblem[] emblems = Array.Empty<Emblem>();

            public PetConfig() { }
            public PetConfig(string name, int hp, params Emblem[] e)
            {
                this.name = name; maxHp = hp; emblems = e;
            }
        }

        /// <summary>
        /// Draw a fixed length emblem array nicely in the inspector (optional nicety).
        /// </summary>
        [AttributeUsage(AttributeTargets.Field)]
        private class EmblemArrayAttribute : PropertyAttribute
        {
            public int Max;
            public EmblemArrayAttribute(int max) { Max = max; }
        }
        #endregion

        private void Start()
        {
            _rng = (rngSeed == 0) ? new Random() : new Random(rngSeed);
            Debug.Log("[BattleManager] Start() initialized.");

            if (autoStart)
            {
                Debug.Log("[BattleManager] Auto-starting battle...");
                BuildParties();
                StartCoroutine(BattleLoop());
            }
        }

        /// <summary>Call this from a Retry button.</summary>
        public void Retry()
        {
            StopAllCoroutines();
            _player.Clear();
            _enemy.Clear();
            BuildParties();
            StartCoroutine(BattleLoop());
        }

        /// <summary>Simple pause: GameManager or a UI can set Time.timeScale = 0/1.</summary>
        public void SetPaused(bool paused) => Time.timeScale = paused ? 0f : 1f;

        private void BuildParties()
        {
            // Player from inspector
            _player.Clear();
            for (int i = 0; i < Mathf.Min(maxPartySize, playerPartyConfig.Count); i++)
            {
                var c = playerPartyConfig[i];
                _player.Add(new Pet(c.name, c.maxHp, ClampEmblems(c.emblems)));
            }
            OnPartyBuilt?.Invoke(Side.Player, _player);

            // Enemy generated
            _enemy.Clear();
            var enemyNames = new[] { "Cricket", "Pig", "Otter", "Beetle", "Crab", "Fish" };
            for (int i = 0; i < maxPartySize; i++)
            {
                var hp   = _rng.Next(enemyHpRange.x, enemyHpRange.y + 1);
                var eCnt = _rng.Next(enemyEmblemCount.x, enemyEmblemCount.y + 1);
                var ems  = new List<Emblem>(eCnt);
                for (int k = 0; k < eCnt; k++) ems.Add((Emblem)_rng.Next(0, 3));
                var name = enemyNames[_rng.Next(enemyNames.Length)];
                _enemy.Add(new Pet(name, hp, ems));
            }
            OnPartyBuilt?.Invoke(Side.Enemy, _enemy);

            OnBattleStart?.Invoke();
        }

        private IEnumerable<Emblem> ClampEmblems(Emblem[] emblems)
        {
            if (emblems == null) yield break;
            int count = Mathf.Min(3, emblems.Length);
            for (int i = 0; i < count; i++) yield return emblems[i];
        }

        private IEnumerator BattleLoop()
        {
            // Continue while both sides have at least one alive pet.
            while (FirstAlive(_player) != null && FirstAlive(_enemy) != null)
            {
                var p1 = FirstAlive(_player);
                var p2 = FirstAlive(_enemy);

                OnDuelStart?.Invoke(p1, p2);

                // Clash until one dies
                while (p1.IsAlive && p2.IsAlive)
                {
                    var e1 = p1.ChooseRandomEmblem(_rng);
                    var e2 = p2.ChooseRandomEmblem(_rng);
                    int cmp = EmblemRules.Compare(e1, e2);

                    // UI tick: show both picks, then apply outcome
                    OnRollResolved?.Invoke(p1, p2, e1, e2, cmp < 0 ? 1 : (cmp > 0 ? 0 : -1));
                    if (rollDelaySeconds > 0f) yield return new WaitForSeconds(rollDelaySeconds);

                    if (cmp > 0)       p2.TakeDamage(damagePerWin); // player wins this roll
                    else if (cmp < 0)  p1.TakeDamage(damagePerWin); // enemy wins
                    else               continue;                    // tie -> reroll

                    if (!p1.IsAlive) HandleDeath(_player, p1);
                    if (!p2.IsAlive) HandleDeath(_enemy,  p2);
                }

                // small spacer between duels so UI can animate units returning
                if (rollDelaySeconds > 0f) yield return new WaitForSeconds(rollDelaySeconds * 0.5f);
            }

            // Someone ran out of alive pets
            var winner = (FirstAlive(_player) != null) ? Side.Player : Side.Enemy;
            OnBattleEnded?.Invoke(winner);
        }

        private Pet FirstAlive(List<Pet> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive) return list[i];
            }
            return null;
        }

        private void HandleDeath(List<Pet> party, Pet pet)
        {
            OnPetDied?.Invoke(pet);

            // Move to end & keep dead (unusable)
            int idx = party.IndexOf(pet);
            if (idx >= 0 && idx < party.Count - 1)
            {
                party.RemoveAt(idx);
                party.Add(pet);
            }
        }
    }
}