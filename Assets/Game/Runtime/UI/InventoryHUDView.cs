using Game.com.game399.shared.Services.Implementation;
using TMPro;
using UnityEngine;

namespace Game.Runtime.UI
{
    public class InventoryHUDView : ObserverMonoBehaviour
    {
        private static InventoryViewModel ViewModel => ServiceResolver.Resolve<InventoryViewModel>();

        [SerializeField] private TextMeshProUGUI foodLabel;
        [SerializeField] private TextMeshProUGUI emblemLabel;
        [SerializeField] private TextMeshProUGUI keyLabel;

        protected override void Subscribe()
        {
            ViewModel.FoodCount.ChangeEvent += OnFoodChanged;
            ViewModel.EmblemCount.ChangeEvent += OnEmblemChanged;
            ViewModel.KeyCount.ChangeEvent += OnKeyChanged;
        }

        protected override void Unsubscribe()
        {
            ViewModel.FoodCount.ChangeEvent -= OnFoodChanged;
            ViewModel.EmblemCount.ChangeEvent -= OnEmblemChanged;
            ViewModel.KeyCount.ChangeEvent -= OnKeyChanged;
        }

        private void OnFoodChanged(int val)   => foodLabel.text = $"x{val}";
        private void OnEmblemChanged(int val) => emblemLabel.text = $"x{val}";
        private void OnKeyChanged(int val)    => keyLabel.text = $"x{val}";
    }
}