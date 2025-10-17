using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Monster")]
public abstract class MonsterClass : ScriptableObject
{
    [Header("Monster")] //data shared across every item
    public string monsterName;
    public Sprite monsterIcon;

    public abstract MonsterClass GetMonster();
    public abstract DesertMonsterClass GetDesertMonster();
    public abstract AetherMonsterClass GetAetherMonster();
    public abstract BossMonsterClass GetBossMonster();
}
