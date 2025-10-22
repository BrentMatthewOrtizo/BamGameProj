using UnityEngine;

[CreateAssetMenu(fileName = "AetherMonster", menuName = "Monster/AetherMonster")]
public class AetherMonsterClass : MonsterClass
{
    [Header("AetherMonster")]
    public AetherMonsterType monsterType;
    
    public enum AetherMonsterType
    {
        SnowFox,
        Yeti
    }
    
    public override MonsterClass GetMonster() { return this; }
    public override DesertMonsterClass GetDesertMonster() { return null; }
    public override AetherMonsterClass GetAetherMonster(){ return this; }
    public override BossMonsterClass GetBossMonster(){ return null; }
}
