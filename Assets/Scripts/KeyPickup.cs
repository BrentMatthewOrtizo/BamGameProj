using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool HasKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasKey = true;
            Destroy(gameObject);
        }
    }
}