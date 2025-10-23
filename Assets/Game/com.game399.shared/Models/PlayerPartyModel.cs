using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.com.game399.shared.Models
{
    /// <summary>
    /// Stores the player's tamed monsters across scenes.
    /// Persists via the MVVM architecture.
    /// </summary>
    [Serializable]
    public class PlayerPartyModel
    {
        public const int MaxPartySize = 3;

        /// <summary>
        /// The player's current party of tamed monsters.
        /// </summary>
        [SerializeField] private List<TamedMonster> party = new();
        public IReadOnlyList<TamedMonster> Party => party;

        /// <summary>
        /// True if the player already has the maximum number of monsters.
        /// </summary>
        public bool IsFull => party.Count >= MaxPartySize;

        /// <summary>
        /// Adds a monster to the player's party (if space available).
        /// </summary>
        public void AddMonster(TamedMonster monster)
        {
            if (monster == null)
            {
                Debug.LogWarning("[PlayerPartyModel] Tried to add a null monster!");
                return;
            }

            if (IsFull)
            {
                Debug.LogWarning("[PlayerPartyModel] Cannot add monster — party is already full!");
                return;
            }

            party.Add(monster);
            Debug.Log($"[PlayerPartyModel] Added {monster.name} (HP {monster.maxHp}, DMG {monster.damage}). Current size: {party.Count}/{MaxPartySize}");
        }

        /// <summary>
        /// Initializes the player's party with the default starter monster if empty.
        /// </summary>
        public void InitializeDefaultPartyIfEmpty()
        {
            if (party.Count == 0)
            {
                party.Add(new TamedMonster("Fox", 5, 2));
                Debug.Log("[PlayerPartyModel] Initialized with starter Fox (5 HP, 2 DMG).");
            }
        }

        /// <summary>
        /// Removes all monsters (for debugging or resets).
        /// </summary>
        public void ClearParty()
        {
            party.Clear();
            Debug.Log("[PlayerPartyModel] Party cleared.");
        }
    }
}