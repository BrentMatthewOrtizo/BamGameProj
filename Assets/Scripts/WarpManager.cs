using UnityEngine;
using UnityEngine.SceneManagement;

public static class WarpManager
{
    public static string previousSceneName;
    public static Vector3 previousPosition;

    public static void SavePlayerPosition(GameObject player)
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        previousPosition = player.transform.position;
    }
}