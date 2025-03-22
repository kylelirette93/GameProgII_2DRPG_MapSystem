using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Inventory system class is responsible for managing the player's inventory.
    /// </summary>
    public class InventorySystem
{
        private List<ItemComponent> items = new List<ItemComponent>();
        private int inventorySlots = 5;

        /// <summary>
        /// Add's an item to the player's inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to add.</param>
        /// <param name="item">The icon for the item to add.</param>
        public void AddItem(ItemComponent item)
        {
            if (items.Count < inventorySlots)
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Removes an item from the player's inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to remove from player's inventory.</param>
        public void RemoveItem(ItemComponent item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
}
}
