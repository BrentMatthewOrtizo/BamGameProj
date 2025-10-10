using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomWarpZone : MonoBehaviour
{
    public int targetSceneIndex = 1;
    
    [Range(0f, 1f)] public float warpChance = 0.3f;  // 30% by default

    public bool oneTimeUse = false;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (other.CompareTag("Player"))
        {
            float roll = Random.value;
            if (roll <= warpChance)
            {
                WarpManager.SavePlayerPosition(other.gameObject);
                SceneManager.LoadScene(targetSceneIndex);

                if (oneTimeUse)
                    used = true;
            }
        }
    }
}