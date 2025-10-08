using Unity.Cinemachine;
using UnityEngine;

public class ForestToDesert : MonoBehaviour, IInteractable
{
    public Transform teleportDestination;
    public PolygonCollider2D newCameraBounds; // Desert bounds

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null && teleportDestination != null)
            {
                player.transform.position = teleportDestination.position;
            }

            CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
            if (confiner != null && newCameraBounds != null)
            {
                confiner.BoundingShape2D = newCameraBounds;
                confiner.InvalidateBoundingShapeCache();
            }
    }
}