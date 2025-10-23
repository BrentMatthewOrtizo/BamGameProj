using System;

namespace AutoBattler
{
    /// <summary>
    /// Defines the available Emblems and their RPS relationships.
    /// </summary>
    public enum Emblem
    {
        Sword = 0,
        Shield = 1,
        Magic = 2
    }

    public static class EmblemRules
    {
        /// <summary>
        /// Compares two emblems using rock-paper-scissors logic.
        /// Returns +1 if 'a' wins, -1 if 'a' loses, 0 for tie.
        /// </summary>
        public static int Compare(Emblem a, Emblem b)
        {
            if (a == b) return 0;

            // Magic > Shield, Shield > Sword, Sword > Magic
            if ((a == Emblem.Magic  && b == Emblem.Shield) ||
                (a == Emblem.Shield && b == Emblem.Sword)  ||
                (a == Emblem.Sword  && b == Emblem.Magic))
                return +1;

            return -1;
        }
    }
}