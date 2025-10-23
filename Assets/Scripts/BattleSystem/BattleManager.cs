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
                var p1 = FirstAlive(_player);
                var p2 = FirstAlive(_enemy);
                OnDuelStart?.Invoke(p1, p2);

                while (p1.IsAlive && p2.IsAlive)
                {
                    // keep rerolling until we get a winner
                    int cmp;
                    Emblem e1;
                    Emblem e2;

                    do
                    {
                        e1 = p1.ChooseRandomEmblem(_rng);
                        e2 = p2.ChooseRandomEmblem(_rng);
                        cmp = EmblemRules.Compare(e1, e2);

                        OnRollResolved?.Invoke(p1, p2, e1, e2, cmp < 0 ? 1 : (cmp > 0 ? 0 : -1));
                        if (rollDelaySeconds > 0f)
                            yield return new WaitForSeconds(rollDelaySeconds);
                    }
                    while (cmp == 0); // reroll until not a tie

                    // apply result
                    if (cmp > 0)
                        p2.TakeDamage(damagePerWin);
                    else if (cmp < 0)
                        p1.TakeDamage(damagePerWin);

                    if (!p1.IsAlive) HandleDeath(_player, p1);
                    if (!p2.IsAlive) HandleDeath(_enemy, p2);
                }

                // small delay before next duel
                if (rollDelaySeconds > 0f)
                    yield return new WaitForSeconds(rollDelaySeconds * 0.5f);
            }

            var winner = (FirstAlive(_player) != null) ? Side.Player : Side.Enemy;
            OnBattleEnded?.Invoke(winner);
        }

        private Pet FirstAlive(List<Pet> list)
        {
            foreach (var pet in list)
                if (pet.IsAlive)
                    return pet;
            return null;
        }

        private void HandleDeath(List<Pet> party, Pet pet)
        {
            OnPetDied?.Invoke(pet);
            int idx = party.IndexOf(pet);
            if (idx >= 0 && idx < party.Count - 1)
            {
                party.RemoveAt(idx);
                party.Add(pet);
            }
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