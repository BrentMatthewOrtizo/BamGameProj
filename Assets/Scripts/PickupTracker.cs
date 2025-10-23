using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class PickupTracker : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static PickupTracker Instance;

    private HashSet<string> collectedIDs = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persist across scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasCollected(string id)
    {
        return collectedIDs.Contains(id);
    }

    public void MarkCollected(string id)
    {
        if (!collectedIDs.Contains(id))
        {
            collectedIDs.Add(id);
            Log.Info($"[PickupTracker] Recorded collected item: {id}");
        }
    }
}