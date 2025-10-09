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
        if (!CanInteract())
        {
            return;
        }
        if (sceneIndexToLoad < 0 || sceneIndexToLoad >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Invalid scene index! Check Build Settings.");
            return;
        }
        SceneManager.LoadScene(sceneIndexToLoad);
    }
}