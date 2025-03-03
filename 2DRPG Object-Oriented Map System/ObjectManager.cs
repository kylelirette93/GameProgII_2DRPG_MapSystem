using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Game Manager class manages all game entities, updating them & drawing them.
    /// </summary>
    public static class ObjectManager
    {
        // List of game objects to update and draw dynamically.
        private static List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// Add Game Object method add's a game object to a list.
        /// </summary>
        /// <param name="obj"></param>
        public static void AddGameObject(GameObject obj)
        {
            if (!gameObjects.Contains(obj))
            {
                gameObjects.Add(obj);
            }
        }
        /// <summary>
        /// Find game object by tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static GameObject Find(string tag)
        {
            return gameObjects.FirstOrDefault(obj => obj.Tag == tag);
        }

        public static Queue<TurnComponent> ReturnAll()
        {
            return new Queue<TurnComponent>(
                gameObjects
                    .Select(obj => obj.GetComponent<TurnComponent>()) // Get TurnComponent from each object
                    .Where(tc => tc != null) // Ensure only valid components are added
                    .Distinct()
            );
        }

        /// <summary>
        /// Update All method updates all game objects at once.
        /// </summary>
        public static void UpdateAll()
        {
            foreach (var obj in gameObjects)
            {
                obj.Update();
            }
        }
        /// <summary>
        /// Draw All method draws all game objects at once.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (var obj in gameObjects)
            {
                obj.Draw(spriteBatch);
            }
        }
    }
}


