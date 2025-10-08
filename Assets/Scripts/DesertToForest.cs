using UnityEngine;

public class DesertToForest : MonoBehaviour, IInteractable
{
    public Transform teleportDestination; // assign in inspector
    private bool isUnlocked => KeyPickup.HasKey; // checks key collected

    public bool CanInteract()
    {
        return isUnlocked; // only allow interaction if key is collected
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        // Teleport player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
            Debug.Log("Teleported to forest!");
        }
    }
}