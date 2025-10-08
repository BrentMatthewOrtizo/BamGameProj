using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
    InventoryManager inventoryManager =  other.GetComponent<InventoryManager>();
    if (inventoryManager)
    {
        bool isPickedUp = inventoryManager.PickupItem(gameObject);
        if (isPickedUp)
        {
            Destroy(gameObject);   
        }
    }
    }
}
