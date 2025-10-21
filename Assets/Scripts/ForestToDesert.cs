using Unity.Cinemachine;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class ForestToDesert : MonoBehaviour, IInteractable
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public Transform teleportDestination;
    public PolygonCollider2D newCameraBounds; // Desert bounds

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        Log.Info("Player interacted with the obelisk.");
        AudioManager.Instance?.PlayObeliskInteractSFX();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
            Log.Info("Player teleported to the desert biome.");
        }

        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner != null && newCameraBounds != null)
        {
            confiner.BoundingShape2D = newCameraBounds;
            confiner.InvalidateBoundingShapeCache();
        }

        AudioManager.Instance?.SwitchBiome("Desert");
    }
}