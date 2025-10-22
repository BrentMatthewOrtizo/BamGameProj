using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButtonHandler : MonoBehaviour
{
    public void OnClick()
    {
        if (!string.IsNullOrEmpty(WarpManager.previousSceneName))
        {
            Debug.Log("Player left the battle scene and returned to previous location.");
            SceneManager.LoadScene(WarpManager.previousSceneName);
        }
    }
}