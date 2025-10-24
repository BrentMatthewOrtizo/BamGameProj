using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class HeavenToStart : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public void BackToStartScene()
    {
        Log.Info("Player has returned back to the Start Scene.");
        SceneManager.LoadSceneAsync(0);
    }
}