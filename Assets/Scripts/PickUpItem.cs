using System;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public ItemClass itemToPickUp;
    private InventoryManager inventoryManager;
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemToPickUp);
            Log.Info($"Player collected {itemToPickUp.itemName}.");
            Log.Info(itemToPickUp + " was added to inventory");
            AudioManager.Instance?.PlayItemPickupSFX();
            Destroy(gameObject);
        }
    }
}
