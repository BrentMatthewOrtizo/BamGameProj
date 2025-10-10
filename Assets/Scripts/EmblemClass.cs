using UnityEngine;

[CreateAssetMenu(fileName = "new Emblem Class", menuName = "Item/Emblem")]
public class EmblemClass : ItemClass
{
    [Header("Emblem")]
    public EmblemType emblemType;
    
    public enum EmblemType
    {
        Magic,
        Sword,
        Shield
    }
    
    public override ItemClass GetItem() { return this; }
    public override EmblemClass GetEmblem() { return this; }
    public override ConsumableClass GetConsumable() { return null; }
}
