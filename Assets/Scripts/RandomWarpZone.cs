using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class RandomWarpZone : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public int targetSceneIndex = 1;
    
    [Range(0f, 1f)] public float warpChance = 0.3f;

    public bool oneTimeUse = false;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (other.CompareTag("Player"))
        {
            float roll = Random.value;
            Log.Info($"Player entered warp zone. Roll = {roll:F2}");

            if (roll <= warpChance)
            {
                Log.Info("Player entered a battle scene.");
                WarpManager.SavePlayerPosition(other.gameObject);
                SceneManager.LoadScene(targetSceneIndex);

                if (oneTimeUse)
                    used = true;
            }
            else
            {
                Log.Info("Warp did not trigger; player continues exploring.");
            }
        }
    }
}