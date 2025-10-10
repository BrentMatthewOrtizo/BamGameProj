using UnityEngine;

[CreateAssetMenu(fileName = "BossMonster", menuName = "Monster/Boss")]
public class BossMonsterClass : MonsterClass
{
    [Header("BossMonster")]
    public BossMonsterType monsterType;
    
    public enum BossMonsterType
    {
        Druid,
        Chimera
    }
    
    public override MonsterClass GetMonster() { return this; }
    public override DesertMonsterClass GetDesertMonster() { return null; }
    public override AetherMonsterClass GetAetherMonster(){ return null; }
    public override BossMonsterClass GetBossMonster(){ return this; }
}
