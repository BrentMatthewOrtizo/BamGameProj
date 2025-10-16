using UnityEngine;
using Unity.Cinemachine;

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
        if (!CanInteract()) return;

        Debug.Log("Player interacted with the obelisk.");
        AudioManager.Instance?.PlayObeliskInteractSFX();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
            Debug.Log("Player teleported to the forest biome.");
        }

        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner != null && newCameraBounds != null)
        {
            confiner.BoundingShape2D = newCameraBounds;
            confiner.InvalidateBoundingShapeCache();
        }

        AudioManager.Instance?.SwitchBiome("Forest");
    }
}