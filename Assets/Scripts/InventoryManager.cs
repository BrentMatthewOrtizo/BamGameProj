using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemClass> items = new List<ItemClass>();
    public ItemClass itemToAdd;
    public ItemClass itemToRemove; //after accessing inventory, select item and set it to this in order to remove it
    public void Start()
    {
        Add(itemToAdd);//would put this when u collide with item or press E
        Remove(itemToRemove); //put here just to test
    }
    
    public void Add(ItemClass item)
    {
        items.Add(item);
    }

    public void Remove(ItemClass item)
    {
        items.Remove(item);
    }
    
    
    // public int itemCount;
    //
    // public bool PickupItem(GameObject item)
    // {
    //     switch (item.tag)
    //     {
    //         case Constants.TAG_FOOD:
    //             itemCount++;
    //             return true;
    //         default:
    //             return false;
    //     }
    //     
    // }
}
