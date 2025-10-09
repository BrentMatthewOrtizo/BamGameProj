using System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public ConsumableClass consumable;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        inventoryManager.Add(consumable); 
        Destroy(gameObject);   
    }
}
