using System;

namespace AutoBattler
{
    /// <summary>
    /// Emblems: duplicates allowed per pet. RPS rules:
    /// Magic > Shield, Shield > Sword, Sword > Magic
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
        /// +1 if a beats b, -1 if a loses to b, 0 if tie
        /// </summary>
        public static int Compare(Emblem a, Emblem b)
        {
            if (a == b) return 0;

            // a beats b cases:
            // Magic > Shield, Shield > Sword, Sword > Magic
            if ((a == Emblem.Magic  && b == Emblem.Shield) ||
                (a == Emblem.Shield && b == Emblem.Sword)  ||
                (a == Emblem.Sword  && b == Emblem.Magic))
            {
                return +1;
            }

            return -1; // otherwise a loses to b
        }

        public static bool Beats(this Emblem a, Emblem b) => Compare(a, b) > 0;
        public static bool LosesTo(this Emblem a, Emblem b) => Compare(a, b) < 0;
    }
}