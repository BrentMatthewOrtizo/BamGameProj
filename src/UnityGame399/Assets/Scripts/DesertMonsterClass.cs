using UnityEngine;

[CreateAssetMenu(fileName = "DesertMonster", menuName = "Monster/DesertMonster")]
public class DesertMonsterClass : MonsterClass
{
    [Header("DesertMonster")]
    public DesertMonsterType monsterType;
    
    public enum DesertMonsterType
    {
        Camel,
        Vulture
    }
    
    public override MonsterClass GetMonster() { return this; }
    public override DesertMonsterClass GetDesertMonster() { return this; }
    public override AetherMonsterClass GetAetherMonster(){ return null; } 
    public override BossMonsterClass GetBossMonster(){ return null; }
}
