using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButtonHandler : MonoBehaviour
{
    public void OnClick()
    {
        if (!string.IsNullOrEmpty(WarpManager.previousSceneName))
        {
            SceneManager.LoadScene(WarpManager.previousSceneName);
        }
    }
}