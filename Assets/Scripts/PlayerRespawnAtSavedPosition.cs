using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnAtSavedPosition : MonoBehaviour
{
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (WarpManager.previousSceneName == currentScene && WarpManager.previousPosition != Vector3.zero)
        {
            transform.position = WarpManager.previousPosition;
        }
    }
}