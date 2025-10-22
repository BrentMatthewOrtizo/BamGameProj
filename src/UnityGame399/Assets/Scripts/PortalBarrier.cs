using UnityEngine;

public class PortalBarrier : MonoBehaviour
{
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (PortalPickup.HasPortalKey && col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }
}