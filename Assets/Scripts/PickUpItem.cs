using System;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public ItemClass itemToPickUp;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        inventoryManager.Add(itemToPickUp); 
        Destroy(gameObject);   
    }
}
