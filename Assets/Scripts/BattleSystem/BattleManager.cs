using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

// add these so we can resolve the MVVM party model
using Game.Runtime;
using Game.com.game399.shared.Models;

namespace AutoBattler
{
    public enum Side { Player, Enemy }

    public class BattleManager : MonoBehaviour
    {
        [Header("Config")]
        [Range(1, 5)] public int maxPartySize = 3;
        public int damagePerWin = 1; // kept for compatibility; not used in calc anymore
        public int rngSeed = 0;
        public bool autoStart = true;

        [Header("Audio")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioClip battleMusic;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.6f;

        [Header("Flow Timing")]
        public float preDuelDelay = 0.3f;
        public float rollTotalDuration = 2.0f;
        public float rollCycleInterval = 1.0f;
        public float postRevealDelay = 0.5f;
        public float postDamageDelay = 2.0f;

        [Header("Enemy Party (Generated)")]
        public Vector2Int enemyHpRange = new(5, 7);        // not used now; kept for inspector
        public Vector2Int enemyEmblemCount = new(1, 3);    // not used now; kept for inspector

        // Runtime lists
        private readonly List<Pet> _player = new();
        private readonly List<Pet> _enemy = new();
        private Random _rng;

        #region Events
        public event Action OnBattleStart;
        public event Action<Side, List<Pet>> OnPartyBuilt;
        public event Action<Pet, Pet> OnDuelStart;
        public event Action<Pet, Pet> OnRollStart;
        public event Action<Pet, Pet, Emblem, Emblem, int> OnRollResolved;
        public event Action<Pet, Pet, int> OnDamageApplied;
        public event Action<Pet> OnPetDied;
        public event Action<Pet, Side> OnPetKilledFinal;
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
            _rng = (rngSeed == 0) ? new Random() : new Random(rngSeed);

            // Play background music
            if (bgmSource && battleMusic)
            {
                bgmSource.clip = battleMusic;
                bgmSource.loop = true;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            }

            if (autoStart)
            {
                BuildParties();
                OnBattleStart?.Invoke();
                StartCoroutine(BattleLoop());
            }
        }

        public void Retry()
        {
            StopAllCoroutines();
            _player.Clear();
            _enemy.Clear();
            BuildParties();
            OnBattleStart?.Invoke();
            StartCoroutine(BattleLoop());
        }

        private void BuildParties()
        {
            _player.Clear();

            // Pull persistent player party (starter Fox 5 HP / 2 DMG is ensured by the model)
            var playerPartyModel = ServiceResolver.Resolve<PlayerPartyModel>();
            playerPartyModel.InitializeDefaultPartyIfEmpty();

            // Convert each tamed monster into a Pet (give them all three emblems for now)
            foreach (var tm in playerPartyModel.Party)
            {
                _player.Add(new Pet(
                    tm.name,
                    tm.maxHp,
                    new[] { Emblem.Sword, Emblem.Shield, Emblem.Magic },
                    tm.damage
                ));
            }

            // Reverse for correct display (rightmost attacks first on player side)
            _player.Reverse();
            OnPartyBuilt?.Invoke(Side.Player, _player);

            // -------- Enemy generation: 1–3 units, HP 1–5, DMG 1–3, 1–3 emblems --------
            _enemy.Clear();
            var enemyNames = new[] { "Camel", "Chimera", "Fox" };
            int enemyCount = _rng.Next(1, 4); // 1..3 inclusive

            for (int i = 0; i < enemyCount; i++)
            {
                string name = enemyNames[_rng.Next(enemyNames.Length)];

                int hp = _rng.Next(1, 6);      // 1–5 HP
                int damage = _rng.Next(1, 4);  // 1–3 DMG

                int eCnt = _rng.Next(1, 4);    // 1–3 emblems
                var ems = new List<Emblem>(eCnt);
                for (int j = 0; j < eCnt; j++)
                    ems.Add((Emblem)_rng.Next(0, 3)); // 0..2 -> Sword/Shield/Magic

                _enemy.Add(new Pet(name, hp, ems, damage));
                Debug.Log($"[Enemy] Spawned {name} (HP {hp}, DMG {damage})");
            }

            OnPartyBuilt?.Invoke(Side.Enemy, _enemy);
        }

        private IEnumerator BattleLoop()
        {
            while (FirstAlive(_player) != null && FirstAlive(_enemy) != null)
            {
                var p1 = FirstAlive(_player, false);
                var p2 = FirstAlive(_enemy, true);
                OnDuelStart?.Invoke(p1, p2);

                yield return new WaitForSeconds(preDuelDelay);

                while (p1.IsAlive && p2.IsAlive)
                {
                    Emblem e1, e2;
                    int cmp;

                    // Begin emblem rolling phase
                    OnRollStart?.Invoke(p1, p2);
                    yield return new WaitForSeconds(rollTotalDuration);

                    // Resolve rolls until non-tie
                    do
                    {
                        e1 = p1.ChooseRandomEmblem(_rng);
                        e2 = p2.ChooseRandomEmblem(_rng);
                        cmp = EmblemRules.Compare(e1, e2);

                        int winnerIndex = (cmp > 0) ? 0 : (cmp < 0) ? 1 : -1;
                        OnRollResolved?.Invoke(p1, p2, e1, e2, winnerIndex);

                        yield return new WaitForSeconds(postRevealDelay);
                    }
                    while (cmp == 0);

                    // Apply results (use each pet's Damage stat)
                    if (cmp > 0)
                    {
                        p2.TakeDamage(p1.Damage);
                        OnDamageApplied?.Invoke(p1, p2, 0);
                        if (!p2.IsAlive)
                        {
                            yield return new WaitForSeconds(0.15f);
                            OnPetKilledFinal?.Invoke(p2, Side.Enemy);
                            HandleDeath(_enemy, p2);
                        }
                    }
                    else if (cmp < 0)
                    {
                        p1.TakeDamage(p2.Damage);
                        OnDamageApplied?.Invoke(p1, p2, 1);
                        if (!p1.IsAlive)
                        {
                            yield return new WaitForSeconds(0.15f);
                            OnPetKilledFinal?.Invoke(p1, Side.Player);
                            HandleDeath(_player, p1);
                        }
                    }

                    yield return new WaitForSeconds(postDamageDelay);
                }

                yield return new WaitForSeconds(preDuelDelay);
            }

            // FIXED WINNER CHECK (prevents Victory flash)
            yield return new WaitForSeconds(0.05f); // allow last deaths to process

            bool playerAlive = FirstAlive(_player) != null;
            bool enemyAlive = FirstAlive(_enemy) != null;

            Side winner;

            if (playerAlive && !enemyAlive)
                winner = Side.Player;
            else if (enemyAlive && !playerAlive)
                winner = Side.Enemy;
            else
                winner = Side.Enemy; // both died = defeat by default

            Debug.Log($"[BattleEnd] Winner = {winner}");
            OnBattleEnded?.Invoke(winner);

            // Fade out BGM smoothly
            if (bgmSource)
                StartCoroutine(FadeOutBgm(1.5f));
        }

        private Pet FirstAlive(List<Pet> list, bool isEnemy = false)
        {
            if (isEnemy)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i].IsAlive) return list[i];
            }
            else
            {
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
                    party.Insert(0, pet);
                else
                    party.Add(pet);
            }

            StartCoroutine(RebuildAfterDelay(party));
        }

        private IEnumerator RebuildAfterDelay(List<Pet> party)
        {
            yield return null;
            var side = (party == _player) ? Side.Player : Side.Enemy;
            OnPartyBuilt?.Invoke(side, party);
        }

        private IEnumerator FadeOutBgm(float duration)
        {
            if (!bgmSource) yield break;
            float startVol = bgmSource.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }
            bgmSource.Stop();
            bgmSource.volume = startVol;
        }
    }
}