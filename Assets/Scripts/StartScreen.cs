using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Player started the game and entered the desert biome.");
        SceneManager.LoadSceneAsync(2);
    }
}
