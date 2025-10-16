using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour, IInteractable
{
    
    public int sceneIndexToLoad;

    public bool CanInteract()
    {
        return PortalPickup.HasPortalKey;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        Debug.Log("Player interacted with the portal and returned to the Start Screen.");
        AudioManager.Instance?.PlayObeliskInteractSFX();

        if (sceneIndexToLoad < 0 || sceneIndexToLoad >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Invalid scene index! Check Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneIndexToLoad);
    }
}