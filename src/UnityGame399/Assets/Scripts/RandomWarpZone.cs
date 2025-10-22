using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomWarpZone : MonoBehaviour
{
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
            Debug.Log($"Player entered warp zone. Roll = {roll:F2}");

            if (roll <= warpChance)
            {
                Debug.Log("Player entered a battle scene.");
                WarpManager.SavePlayerPosition(other.gameObject);
                SceneManager.LoadScene(targetSceneIndex);

                if (oneTimeUse)
                    used = true;
            }
            else
            {
                Debug.Log("Warp did not trigger; player continues exploring.");
            }
        }
    }
}