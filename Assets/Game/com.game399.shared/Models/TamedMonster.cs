using System;

namespace Game.com.game399.shared.Models
{
    /// <summary>
    /// A simple, persistent record of a monster the player has tamed.
    /// Emblems/equipment removed for now.
    /// </summary>
    [Serializable]
    public class TamedMonster
    {
        // Species identifier (e.g., "Fox", "Camel", "Chimera")
        public string name;

        // Stats as they should be used when this monster is brought into battle
        public int maxHp;
        public int damage;

        public TamedMonster(string name, int maxHp, int damage)
        {
            this.name = name;
            this.maxHp = maxHp;
            this.damage = damage;
        }

        public override string ToString() => $"{name} (HP {maxHp}, DMG {damage})";
    }
}