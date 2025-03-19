using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Inventory system class is responsible for managing the player's inventory.
    /// </summary>
    public class InventorySystem
{
        private Dictionary<string, Sprite> items = new Dictionary<string, Sprite>();
        private int inventorySlots = 5;

        /// <summary>
        /// Add's an item to the player's inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to add.</param>
        /// <param name="item">The icon for the item to add.</param>
        public void AddItem(string itemName, Sprite item)
        {
            if (items.Count < inventorySlots)
            {
                items.Add(itemName, item);
            }
        }

        /// <summary>
        /// Removes an item from the player's inventory.
        /// </summary>
        /// <param name="itemName">The name of the item to remove from player's inventory.</param>
        public void RemoveItem(string itemName)
        {
            if (items.ContainsKey(itemName))
            {
                items.Remove(itemName);
            }
        }
}
}
