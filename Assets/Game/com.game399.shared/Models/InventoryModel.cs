using Game399.Shared;

namespace Game.com.game399.shared.Models
{
    public class InventoryModel
    {
        public ObservableValue<int> FoodCount { get; } = new(0);
        public ObservableValue<int> EmblemCount { get; } = new(0);
        public ObservableValue<int> KeyCount { get; } = new(0);
    }
}