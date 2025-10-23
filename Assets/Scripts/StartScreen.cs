using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class StartScreen : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public void PlayGame()
    {
        Log.Info("Player started the game and entered the desert biome.");
        SceneManager.LoadSceneAsync(2);
        AudioManager.Instance?.PlayStartScreenButton();
    }
}
