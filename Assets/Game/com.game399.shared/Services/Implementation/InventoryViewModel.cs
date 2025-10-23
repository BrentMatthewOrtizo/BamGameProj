using Game399.Shared;
using Game399.Shared.Models;

namespace Game.com.game399.shared.Services.Implementation
{
    public class InventoryViewModel
    {
        private readonly Models.InventoryModel _model;

        public InventoryViewModel(Models.InventoryModel model)
        {
            _model = model;
        }

        public ObservableValue<int> FoodCount => _model.FoodCount;
        public ObservableValue<int> EmblemCount => _model.EmblemCount;
        public ObservableValue<int> KeyCount => _model.KeyCount;

        public void AddFood(int amount = 1) => _model.FoodCount.Value += amount;
        public void AddEmblem(int amount = 1) => _model.EmblemCount.Value += amount;
        public void AddKey(int amount = 1) => _model.KeyCount.Value += amount;
    }
}