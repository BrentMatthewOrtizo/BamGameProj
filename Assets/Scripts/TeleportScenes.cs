using UnityEngine;
using UnityEngine.SceneManagement; 
using Game399.Shared.Diagnostics;
using Game.Runtime;

// I edited DesertToForest but I know manny did too so I put my scene changing logic with a key to a new class
public class TeleportScenes : MonoBehaviour//, IInteractable
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    //Teleports user to next scene/biome
    private Scene currentScene;
    private int sceneIndex; 
    private bool isUnlocked => KeyPickup.HasKey; // checks key collected
    
    void Start()
    {
        currentScene =  SceneManager.GetActiveScene();
        sceneIndex =  currentScene.buildIndex;
        Log.Info("currently in Scene " + sceneIndex);
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
        Log.Info("Teleported to scene " + sceneIndex);
        
    }
    
}