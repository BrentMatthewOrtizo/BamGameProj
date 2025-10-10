using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButtonHandler : MonoBehaviour
{
    // Called by the UI Button's OnClick event
    public void OnClick()
    {
        if (!string.IsNullOrEmpty(WarpManager.previousSceneName))
        {
            SceneManager.LoadScene(WarpManager.previousSceneName);
        }
    }
}