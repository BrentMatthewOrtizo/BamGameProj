using System.Collections.Generic;
using UnityEngine;

namespace AutoBattler
{
    /// <summary>
    /// Represents a single pet/monster in battle.
    /// </summary>
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

        public void TakeDamage(int amount)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Max(0, amount));
        }

        public Emblem ChooseRandomEmblem(System.Random rng)
        {
            if (Emblems == null || Emblems.Count == 0) return Emblem.Sword;
            return Emblems[rng.Next(Emblems.Count)];
        }
    }
}