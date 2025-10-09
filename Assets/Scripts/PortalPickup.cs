using UnityEngine;

public class PortalPickup : MonoBehaviour
{
    public static bool HasPortalKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HasPortalKey = true;
            Destroy(gameObject);
        }
    }
}
