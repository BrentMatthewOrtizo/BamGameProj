using Game.com.game399.shared.Models;

namespace Game399.Tests
{
    [TestFixture]
    public class InventoryModelTests
    {
        [Test]
        public void InventoryModel_InitialValues_AreZero()
        {
            // Arrange
            var inventory = new InventoryModel();

            // Assert initial values
            Assert.That(0, Is.EqualTo(inventory.FoodCount.Value), "FoodCount should start at 0");
            Assert.That(0, Is.EqualTo(inventory.EmblemCount.Value), "EmblemCount should start at 0");
            Assert.That(0, Is.EqualTo(inventory.KeyCount.Value), "KeyCount should start at 0");
        }

        [Test]
        public void InventoryModel_SetValues_UpdatesCorrectly()
        {
            // Arrange
            var inventory = new InventoryModel();

            // Act
            inventory.FoodCount.Value = 5;
            inventory.EmblemCount.Value = 2;
            inventory.KeyCount.Value = 1;

            // Assert
            Assert.AreEqual(5, inventory.FoodCount.Value, "FoodCount did not update correctly");
            Assert.AreEqual(2, inventory.EmblemCount.Value, "EmblemCount did not update correctly");
            Assert.AreEqual(1, inventory.KeyCount.Value, "KeyCount did not update correctly");
        }

        [Test]
        public void InventoryModel_ChangeEvent_InvokedOnValueChange()
        {
            // Arrange
            var inventory = new InventoryModel();
            int foodEventValue = -1;
            int emblemEventValue = -1;
            int keyEventValue = -1;

            inventory.FoodCount.ChangeEvent += val => foodEventValue = val;
        }
    }
}