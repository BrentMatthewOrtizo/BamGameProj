using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemClass> items = new List<ItemClass>();
    public List<MonsterClass> monsters = new List<MonsterClass>();
    
    public ItemClass itemToAdd;
    public ItemClass itemToRemove; //after accessing inventory, select item and set it to this in order to remove it
    public MonsterClass monsterToAdd;
    public MonsterClass monsterToRemove;
    public void Start()
    {
        AddItem(itemToAdd);//would put this when u collide with item or press E
        RemoveItem(itemToRemove); //put here just to test

        AddMonster(monsterToAdd);
        RemoveMonster(monsterToRemove);
    }
    
    public void AddItem(ItemClass item)
    {
        items.Add(item);
    }
    public void AddMonster(MonsterClass monster)
    {
        monsters.Add(monster);
    }

    public void RemoveItem(ItemClass item)
    {
        items.Remove(item);
    }
    public void RemoveMonster(MonsterClass monster)
    {
        monsters.Remove(monster);
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
