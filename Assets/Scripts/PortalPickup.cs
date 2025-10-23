using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;
using Game.com.game399.shared.Services.Implementation;

public class PortalPickup : MonoBehaviour
{
    public string uniqueID;
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static bool HasPortalKey = false;
    private InventoryViewModel inventoryViewModel;

    private void Start()
    {
        // Despawn if already collected
        if (PickupTracker.Instance != null && PickupTracker.Instance.HasCollected(uniqueID))
        {
            Destroy(gameObject);
            return;
        }

        inventoryViewModel = ServiceResolver.Resolve<InventoryViewModel>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasPortalKey = true;
            Log.Info("Player collected a portal key.");
            inventoryViewModel.AddKey(1);
            AudioManager.Instance?.PlayItemPickupSFX();

            PickupTracker.Instance?.MarkCollected(uniqueID); // mark as collected

            Destroy(gameObject);
        }
    }
}