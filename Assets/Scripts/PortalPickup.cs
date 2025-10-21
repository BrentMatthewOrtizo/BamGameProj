using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class PortalPickup : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static bool HasPortalKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasPortalKey = true;
            Log.Info("Player collected a portal key.");
            AudioManager.Instance?.PlayItemPickupSFX();
            Destroy(gameObject);
        }
    }
}
