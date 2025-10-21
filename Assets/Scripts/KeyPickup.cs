using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class KeyPickup : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static bool HasKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasKey = true;
            Log.Info("Player collected a key.");
            AudioManager.Instance?.PlayItemPickupSFX();
            Destroy(gameObject);
        }
    }
}