using UnityEngine;

public class BossRespawnManager : MonoBehaviour
{
    private void Start()
    {
        // Only apply if we’ve just finished a boss battle
        if (!BossBattleSettings.IsBossBattle && !string.IsNullOrEmpty(BossBattleSettings.LastBossID))
        {
            string defeatedBossName = BossBattleSettings.LastBossID;
            GameObject bossObj = GameObject.Find(defeatedBossName);

            if (bossObj != null)
            {
                // Disable interaction
                var encounter = bossObj.GetComponent<BossEncounter>();
                if (encounter != null)
                    encounter.enabled = false;

                // Make intangible (so player can walk through)
                var col = bossObj.GetComponent<Collider2D>();
                if (col != null)
                    col.isTrigger = true;

                Debug.Log($"[BossRespawnManager] Boss '{defeatedBossName}' is now intangible and uninteractable.");
            }

            // Reset after applying
            BossBattleSettings.Reset();
        }
    }
}