using Game.com.game399.shared.Services.Implementation;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;

namespace AutoBattler
{
    public class GameServiceBridge : MonoBehaviour
    {
        public static GameServiceBridge Instance { get; private set; }

        public InventoryManager InventoryManager { get; private set; }
        public InventoryViewModel InventoryViewModel { get; private set; }

        private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            try
            {
                InventoryManager = GameObject.FindGameObjectWithTag("Inventory")?.GetComponent<InventoryManager>();
                InventoryViewModel = ServiceResolver.Resolve<InventoryViewModel>();

                if (InventoryManager == null)
                    Log?.Warn("[GameServiceBridge] InventoryManager not found in scene.");

                if (InventoryViewModel == null)
                    Log?.Warn("[GameServiceBridge] InventoryViewModel not resolved from ServiceResolver.");
            }
            catch
            {
                Log?.Warn("[GameServiceBridge] Failed to connect services. MVVM might not be initialized yet.");
            }
        }
    }
}