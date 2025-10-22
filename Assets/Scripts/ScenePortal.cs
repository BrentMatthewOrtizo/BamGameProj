using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class ScenePortal : MonoBehaviour, IInteractable
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public int sceneIndexToLoad;

    public bool CanInteract()
    {
        return PortalPickup.HasPortalKey;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        Log.Info("Player interacted with the portal and returned to the Start Screen.");
        AudioManager.Instance?.PlayObeliskInteractSFX();

        if (sceneIndexToLoad < 0 || sceneIndexToLoad >= SceneManager.sceneCountInBuildSettings)
        {
            Log.Warn("Invalid scene index! Check Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneIndexToLoad);
    }
}