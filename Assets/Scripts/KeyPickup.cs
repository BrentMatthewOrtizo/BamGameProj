using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool HasKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasKey = true;
            Debug.Log("Player collected a key.");
            AudioManager.Instance?.PlayItemPickupSFX();
            Destroy(gameObject);
        }
    }
}