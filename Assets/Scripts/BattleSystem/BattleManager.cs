using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace AutoBattler
{
    public enum Side { Player, Enemy }

    /// <summary>
    /// Handles battle flow, logic, and events.
    /// Attach this to an empty GameObject in your scene.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("Config")]
        [Range(1, 5)] public int maxPartySize = 3;
        public int damagePerWin = 1;
        [Range(0f, 1f)] public float rollDelaySeconds = 0.35f;
        public int rngSeed = 0;
        public bool autoStart = true;

        [Header("Player Party (Inspector)")]
        public List<PetConfig> playerPartyConfig = new()
        {
            new PetConfig("Fox", 5,  Emblem.Sword, Emblem.Sword),
            new PetConfig("Camel", 6, Emblem.Shield),
            new PetConfig("Chimera", 7, Emblem.Magic, Emblem.Magic),
        };

        [Header("Enemy Party (Generated)")]
        public Vector2Int enemyHpRange = new(5, 7);
        public Vector2Int enemyEmblemCount = new(1, 3);

        // Runtime state
        private readonly List<Pet> _player = new();
        private readonly List<Pet> _enemy = new();
        private Random _rng;

        #region Events
        public event Action OnBattleStart;
        public event Action<Side, List<Pet>> OnPartyBuilt;
        public event Action<Pet, Pet> OnDuelStart;
        public event Action<Pet, Pet, Emblem, Emblem, int> OnRollResolved;
        public event Action<Pet> OnPetDied;
        public event Action<Side> OnBattleEnded;
        #endregion

        [Serializable]
        public class PetConfig
        {
            public string name = "Pet";
            public int maxHp = 6;
            public Emblem[] emblems = Array.Empty<Emblem>();

            public PetConfig() { }
            public PetConfig(string name, int hp, params Emblem[] e)
            {
                this.name = name;
                maxHp = hp;
                emblems = e;
            }
        }

        private void Start()
        {
            Debug.Log("[BattleManager] Start() running...");
            _rng = (rngSeed == 0) ? new Random() : new Random(rngSeed);

            if (autoStart)
            {
                BuildParties();
                StartCoroutine(BattleLoop());
            }
        }

        public void Retry()
        {
            StopAllCoroutines();
            _player.Clear();
            _enemy.Clear();
            BuildParties();
            StartCoroutine(BattleLoop());
        }

        public void SetPaused(bool paused) => Time.timeScale = paused ? 0f : 1f;

        private void BuildParties()
        {
            _player.Clear();
            for (int i = 0; i < Mathf.Min(maxPartySize, playerPartyConfig.Count); i++)
            {
                var c = playerPartyConfig[i];
                _player.Add(new Pet(c.name, c.maxHp, c.emblems));
            }

            // Reverse so the *last* item (rightmost) is the first attacker
            _player.Reverse();
            OnPartyBuilt?.Invoke(Side.Player, _player);

            _enemy.Clear();
            var names = new[] { "Cricket", "Pig", "Otter", "Beetle", "Crab", "Fish" };
            for (int i = 0; i < maxPartySize; i++)
            {
                int hp = _rng.Next(enemyHpRange.x, enemyHpRange.y + 1);
                int eCnt = _rng.Next(enemyEmblemCount.x, enemyEmblemCount.y + 1);
                var ems = new List<Emblem>(eCnt);
                for (int j = 0; j < eCnt; j++) ems.Add((Emblem)_rng.Next(0, 3));
                string name = names[_rng.Next(names.Length)];
                _enemy.Add(new Pet(name, hp, ems));
            }
            OnPartyBuilt?.Invoke(Side.Enemy, _enemy);

            OnBattleStart?.Invoke();
        }

        private IEnumerator BattleLoop()
        {
            while (FirstAlive(_player) != null && FirstAlive(_enemy) != null)
            {
                var p1 = FirstAlive(_player, false); // Player frontmost
                var p2 = FirstAlive(_enemy, true);   // Enemy frontmost
                OnDuelStart?.Invoke(p1, p2);

                // Small pause before duel starts (for visual clarity)
                yield return new WaitForSeconds(0.6f);

                while (p1.IsAlive && p2.IsAlive)
                {
                    Emblem e1, e2;
                    int cmp;
                    int tieCount = 0; // <-- NEW: track ties

                    do
                    {
                        e1 = p1.ChooseRandomEmblem(_rng);
                        e2 = p2.ChooseRandomEmblem(_rng);
                        cmp = EmblemRules.Compare(e1, e2);

                        // -1 = tie, 0 = player wins, 1 = enemy wins
                        int result = cmp > 0 ? 0 : (cmp < 0 ? 1 : -1);
                        OnRollResolved?.Invoke(p1, p2, e1, e2, result);

                        // Pause for players to see the emblems
                        yield return new WaitForSeconds(0.7f);

                        if (cmp == 0)
                        {
                            tieCount++;
                            // After 3 ties, pick random winner to avoid freeze
                            if (tieCount >= 3)
                            {
                                cmp = (_rng.Next(2) == 0) ? 1 : -1; // random winner
                                Debug.LogWarning($"[BattleManager] Forced winner after 3 ties!");
                            }
                        }

                    } while (cmp == 0); // reroll until a winner

                    // Apply damage
                    if (cmp > 0) // Player wins
                        p2.TakeDamage(damagePerWin);
                    else if (cmp < 0) // Enemy wins
                        p1.TakeDamage(damagePerWin);

                    if (!p1.IsAlive) HandleDeath(_player, p1);
                    if (!p2.IsAlive) HandleDeath(_enemy, p2);

                    // Small pause between rolls inside same duel
                    yield return new WaitForSeconds(0.5f);
                }

                // Small pause between duels
                yield return new WaitForSeconds(0.8f);
            }

            var winner = (FirstAlive(_player) != null) ? Side.Player : Side.Enemy;
            OnBattleEnded?.Invoke(winner);
        }

        private Pet FirstAlive(List<Pet> list, bool isEnemy = false)
        {
            if (isEnemy)
            {
                // Enemy: leftmost to right (closest to center first)
                for (int i = 0; i < list.Count; i++)
                    if (list[i].IsAlive) return list[i];
            }
            else
            {
                // Player: rightmost to left (closest to center first)
                for (int i = list.Count - 1; i >= 0; i--)
                    if (list[i].IsAlive) return list[i];
            }
            return null;
        }

        private void HandleDeath(List<Pet> party, Pet pet)
        {
            OnPetDied?.Invoke(pet);

            if (party.Remove(pet))
            {
                if (party == _player)
                    party.Insert(0, pet); // Player: dead pets move far left
                else
                    party.Add(pet);       // Enemy: dead pets move far right
            }

            StartCoroutine(RebuildAfterDelay(party));
        }

        private IEnumerator RebuildAfterDelay(List<Pet> party)
        {
            yield return null; // one frame delay for UI rebuild safety
            var side = (party == _player) ? Side.Player : Side.Enemy;
            OnPartyBuilt?.Invoke(side, party);
        }

        private void OnEnable()
        {
            OnBattleStart += () => Debug.Log("Battle started!");
            OnPartyBuilt += (side, pets) => Debug.Log($"{side} party built with {pets.Count} pets.");
            OnDuelStart += (p1, p2) => Debug.Log($"Duel Start: {p1.Name} vs {p2.Name}");
            OnRollResolved += (p1, p2, e1, e2, result) =>
                Debug.Log($"Roll: {p1.Name} [{e1}] vs {p2.Name} [{e2}] → {(result == 0 ? "P1 Wins" : result == 1 ? "P2 Wins" : "Tie")}");
            OnPetDied += p => Debug.Log($"{p.Name} has fallen!");
            OnBattleEnded += winner => Debug.Log($"Battle ended — Winner: {winner}");
        }
    }
}