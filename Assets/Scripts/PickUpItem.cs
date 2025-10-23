using System;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using Game.com.game399.shared.Services.Implementation;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public string uniqueID; // used for persistence

    public ItemClass itemToPickUp;
    private InventoryManager inventoryManager;
    private InventoryViewModel inventoryViewModel;
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    void Start()
    {
        // Check if this item was already collected before
        if (PickupTracker.Instance != null && PickupTracker.Instance.HasCollected(uniqueID))
        {
            Destroy(gameObject);
            return;
        }

        inventoryManager = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
        inventoryViewModel = ServiceResolver.Resolve<InventoryViewModel>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemToPickUp);
            Log.Info($"Player collected {itemToPickUp.itemName}.");
            
            if (itemToPickUp is EmblemClass)
            {
                inventoryViewModel.AddEmblem(1);
            }
            else if (itemToPickUp.itemName.ToLower().Contains("meat"))
            {
                inventoryViewModel.AddFood(1);
            }

            AudioManager.Instance?.PlayItemPickupSFX();

            // Mark as collected so it won’t respawn
            PickupTracker.Instance?.MarkCollected(uniqueID);

            Destroy(gameObject);
        }
    }
}