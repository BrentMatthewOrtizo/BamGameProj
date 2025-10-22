using UnityEngine;

[CreateAssetMenu(fileName = "new Consumable Class", menuName = "Item/Consumable")]
public class ConsumableClass : ItemClass
{
    [Header("Consumable")] 
    public float healthAdded; //use to upgrade max health
    
    public ConsumableType consumableType;
    
    public enum ConsumableType
    {
        Meat,
        Potion
    }

    public override ItemClass GetItem() { return this; }
    public override ConsumableClass GetConsumable() { return this; }
    public override EmblemClass GetEmblem() { return null; }
    
}
