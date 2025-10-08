using UnityEngine;
using UnityEngine.SceneManagement; //make 

public class DesertToForest : MonoBehaviour, IInteractable
{
    //Teleports user to next scene/biome
    private Scene currentScene;
    private int sceneIndex; 
    private bool isUnlocked => KeyPickup.HasKey; // checks key collected

    void Start()
    {
        currentScene =  SceneManager.GetActiveScene();
        sceneIndex =  currentScene.buildIndex;
        Debug.Log("currently in Scene " + sceneIndex);
    }
    public bool CanInteract()
    {
        return isUnlocked; // only allow interaction if key is collected
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        // Teleport player to next biome
        sceneIndex++;
        SceneManager.LoadSceneAsync(sceneIndex);
        Debug.Log("Teleported to scene " + sceneIndex);
        
    }
    
}