using System;
using System.Collections.Generic;

namespace Game.com.game399.shared.Models
{
    /// <summary>
    /// Stores the player's tamed monsters across scenes.
    /// </summary>
    [Serializable]
    public class PlayerPartyModel
    {
        public const int MaxPartySize = 3;

        /// <summary>
        /// The player's current party of tamed monsters.
        /// </summary>
        public List<TamedMonster> Party { get; private set; } = new();

        /// <summary>
        /// True if the player already has 3 monsters.
        /// </summary>
        public bool IsFull => Party.Count >= MaxPartySize;

        /// <summary>
        /// Adds a monster to the player's party (if space available).
        /// </summary>
        public void AddMonster(TamedMonster monster)
        {
            if (IsFull)
            {
                UnityEngine.Debug.LogWarning("Cannot add monster — party is already full!");
                return;
            }

            Party.Add(monster);
            UnityEngine.Debug.Log($"[Party] Added {monster.name}! Current size: {Party.Count}");
        }

        /// <summary>
        /// Initializes the player's party with the default starter monster.
        /// </summary>
        public void InitializeDefaultPartyIfEmpty()
        {
            if (Party.Count == 0)
            {
                Party.Add(new TamedMonster("Fox", 5, 1)); // starter stats
                UnityEngine.Debug.Log("[Party] Initialized with starter Fox.");
            }
        }
    }
}