using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace AutoBattler
{
    public enum Side { Player, Enemy }

    public class BattleManager : MonoBehaviour
    {
        [Header("Config")]
        [Range(1, 5)] public int maxPartySize = 3;
        public int damagePerWin = 1;
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
        public event Action<Pet, Side> OnPetKilledFinal; // plays death sound correctly
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

        private void BuildParties()
        {
            _player.Clear();
            for (int i = 0; i < Mathf.Min(maxPartySize, playerPartyConfig.Count); i++)
            {
                var c = playerPartyConfig[i];
                _player.Add(new Pet(c.name, c.maxHp, c.emblems));
            }

            // Reverse order so rightmost pet attacks first
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
                var p1 = FirstAlive(_player, false);
                var p2 = FirstAlive(_enemy, true);
                OnDuelStart?.Invoke(p1, p2);

                yield return new WaitForSeconds(preDuelDelay);

                while (p1.IsAlive && p2.IsAlive)
                {
                    Emblem e1, e2;
                    int cmp;

                    // Begin emblem spin for both pets
                    OnRollStart?.Invoke(p1, p2);
                    yield return new WaitForSeconds(rollTotalDuration);

                    // Keep rerolling until there’s a winner
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

                    // Apply damage + delay
                    if (cmp > 0)
                    {
                        p2.TakeDamage(damagePerWin);
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
                        p1.TakeDamage(damagePerWin);
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

            var winner = (FirstAlive(_player) != null) ? Side.Player : Side.Enemy;
            OnBattleEnded?.Invoke(winner);
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
        
        private void OnEnable()
        {
            OnBattleEnded += winner =>
            {
                if (bgmSource)
                {
                    StartCoroutine(FadeOutBgm(1.5f));
                }
            };
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
            bgmSource.volume = startVol; // reset for next play
        }
    }
}