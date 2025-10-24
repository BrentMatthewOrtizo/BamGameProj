using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Runtime;
using Game399.Shared.Diagnostics;

public class BossEncounter : MonoBehaviour, IInteractable
{
    [Header("Scene to load for battle")]
    [SerializeField] private string battleSceneName = "AutobattlePrototype";

    [Header("Boss Enemy Stats (Range)")]
    public Vector2Int bossHpRange = new Vector2Int(3, 7);
    public Vector2Int bossDamageRange = new Vector2Int(1, 3);

    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public bool CanInteract() => true;

    public void Interact()
    {
        Log.Info($"[BossEncounter] Player interacted with {gameObject.name} — starting boss battle.");

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            WarpManager.SavePlayerPosition(player);

        // Save boss info for return
        BossBattleSettings.SetBoss(gameObject.name, bossHpRange, bossDamageRange);

        SceneManager.LoadScene(battleSceneName);
    }
}