using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public static class GameManager
    {
        private static List<GameObject> gameObjects = new List<GameObject>();

        public static void AddGameObject(GameObject obj)
        {
            gameObjects.Add(obj);
        }

        public static GameObject Find(string tag)
        {
            return gameObjects.FirstOrDefault(obj => obj.Tag == tag);
        }

        public static void UpdateAll()
        {
            foreach (var obj in gameObjects)
            {
                obj.Update();
            }
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (var obj in gameObjects)
            {
                obj.Draw(spriteBatch);
            }
        }
    }
}


