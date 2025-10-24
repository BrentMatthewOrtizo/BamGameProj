using UnityEngine;

public static class BossBattleSettings
{
    public static bool IsBossBattle = false;
    public static Vector2Int BossHpRange = new Vector2Int(3, 7);
    public static Vector2Int BossDamageRange = new Vector2Int(1, 3);

    // Track which boss was fought
    public static string LastBossID = "";

    public static void SetBoss(string bossID, Vector2Int hpRange, Vector2Int dmgRange)
    {
        IsBossBattle = true;
        LastBossID = bossID;
        BossHpRange = hpRange;
        BossDamageRange = dmgRange;
    }

    public static void Reset()
    {
        IsBossBattle = false;
        LastBossID = "";
    }
}