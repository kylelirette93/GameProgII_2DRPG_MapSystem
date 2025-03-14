using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
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
        private static List<GameObject> toRemove = new List<GameObject>();
        private static List<GameObject> toAdd = new List<GameObject>();

        /// <summary>
        /// Add Game Object method add's a game object to a list.
        /// </summary>
        /// <param name="obj"></param>
        public static void AddGameObject(GameObject obj)
        {
            if (!gameObjects.Contains(obj) && !toAdd.Contains(obj))
            {
                toAdd.Add(obj);
                Debug.WriteLine("Adding game object: " + obj.Tag);
            }
        }

        public static void RemoveGameObject(GameObject obj)
        {
            if (gameObjects.Contains(obj)) {
                toRemove.Add(obj);
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

        public static List<GameObject> FindAllObjectsByTag(string tagPrefix)
        {
            return gameObjects.Where(obj => obj.Tag.StartsWith(tagPrefix)).ToList();
        }

        public static Queue<TurnComponent> ReturnAll()
        {
            var turnComponentQueue = new Queue<TurnComponent>(
                gameObjects
                    .Select(obj =>
                    {
                        var component = obj.GetComponent<TurnComponent>();
                        //Debug.WriteLine($"Selecting GameObject: {obj.Tag}, TurnComponent: {component}");
                        return component;
                    })
                    .Where(tc =>
                    {
                        return tc != null;
                    })
            );

            return turnComponentQueue;
        }

        public static List<GameObject> FindAll()
        {
            return gameObjects;
        }

        /// <summary>
        /// Update All method updates all game objects at once.
        /// </summary>
        public static void UpdateAll()
        {
            // Remove objects
            foreach (var obj in toRemove)
            {
                if (obj != null && gameObjects.Contains(obj))
                {
                    gameObjects.Remove(obj);
                    //Debug.WriteLine($"Removed: {obj.Tag}, gameObjects count: {gameObjects.Count}");
                }
            }
            toRemove.Clear();

            // Add objects
            foreach (var obj in toAdd)
            {
                if (obj != null && !gameObjects.Contains(obj))
                {
                    gameObjects.Add(obj);
                    //Debug.WriteLine($"Added: {obj.Tag}, gameObjects count: {gameObjects.Count}");
                }
            }
            toAdd.Clear();

            // Iterate over a copy for updating
            var gameObjectsCopy = new List<GameObject>(gameObjects); // Create a copy
            //Debug.WriteLine($"gameObjects count: {gameObjects.Count}");
            foreach (var obj in gameObjectsCopy)
            {
                if (obj != null)
                {
                    //Debug.WriteLine($"Updating: {obj.Tag}");
                    obj.Update();
                    //Debug.WriteLine($"Updated: {obj.Tag}");
                }
                else
                {
                    //Debug.WriteLine("Null GameObject found in gameObjects!");
                }
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


