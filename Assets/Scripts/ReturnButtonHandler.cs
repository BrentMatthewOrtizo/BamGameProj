using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class ReturnButtonHandler : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public void OnClick()
    {
        if (!string.IsNullOrEmpty(WarpManager.previousSceneName))
        {
            Log.Info("Player left the battle scene and returned to previous location.");
            SceneManager.LoadScene(WarpManager.previousSceneName);
        }
    }
}