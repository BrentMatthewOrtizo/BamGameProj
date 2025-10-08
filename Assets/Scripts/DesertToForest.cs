using UnityEngine;
using Unity.Cinemachine; // <-- make sure to include this!

public class DesertToForest : MonoBehaviour, IInteractable
{
    public Transform teleportDestination; // Where the player will teleport to
    public PolygonCollider2D newCameraBounds; // Assign your new area’s collider in Inspector

    private bool isUnlocked => KeyPickup.HasKey;

    public bool CanInteract()
    {
        return isUnlocked;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        // Teleport player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
        }

        // Find the Cinemachine camera and update its confiner
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (confiner != null && newCameraBounds != null)
        {
            confiner.BoundingShape2D = newCameraBounds;
            confiner.InvalidateBoundingShapeCache();
        }
    }
}