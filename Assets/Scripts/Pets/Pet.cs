using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattler
{
    [Serializable]
    public class Pet
    {
        public string Name;
        public int MaxHP;
        public int CurrentHP;
        public List<Emblem> Emblems = new();

        public bool IsAlive => CurrentHP > 0;

        public Pet(string name, int maxHp, IEnumerable<Emblem> emblems)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Pet" : name.Trim();
            MaxHP = Mathf.Max(1, maxHp);
            CurrentHP = MaxHP;
            if (emblems != null) Emblems.AddRange(emblems);
        }

        public void Reset()
        {
            CurrentHP = MaxHP;
        }

        public void TakeDamage(int amount)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Max(0, amount));
        }

        /// <summary>
        /// Randomly pick any of this pet's equipped emblems (duplicates allowed).
        /// If the pet has no emblems, default to Sword so the duel can still resolve.
        /// </summary>
        public Emblem ChooseRandomEmblem(System.Random rng)
        {
            if (Emblems == null || Emblems.Count == 0) return Emblem.Sword;
            var i = rng.Next(Emblems.Count); // inclusive min, exclusive max
            return Emblems[i];
        }

        public override string ToString() => $"{Name} ({CurrentHP}/{MaxHP})";
    }
}