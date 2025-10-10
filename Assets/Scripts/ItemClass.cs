using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public abstract class ItemClass : ScriptableObject
{
    [Header("Item")] //data shared across every item
    public string itemName;
    public Sprite itemIcon;

    public abstract ItemClass GetItem();
    public abstract EmblemClass GetEmblem();
    public abstract ConsumableClass GetConsumable();
    
}
