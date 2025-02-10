using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Map Manager class is responsible for loading map files based on an index.
    /// </summary>
    public class MapManager
{
        public Vector2 SpawnPoint { get; set; }
        private Vector2 _spawnPoint;
        private SpriteBatch _spriteBatch;
        List<string> levels = new List<string>();
        private int currentLevelIndex;

        /// <summary>
        /// Constructor that loads map files.
        /// </summary>
        public MapManager()
        {
            currentLevelIndex = 0;
            LoadMaps();
        }

        public Tilemap map { get; set; }

        /// <summary>
        ///  Load maps concatenate's a string with the index of the map list and adds file paths according to the index to the list.
        /// </summary>
        public void LoadMaps()
        {
            for (int i = 0; i < 3; i++)
            {
                string levelPath = $"Content/Level{i}.txt";
                if (File.Exists(levelPath))
                {
                    levels.Add(levelPath);
                }
                else
                {
                    Console.WriteLine($"Warning: Level file {levelPath} does not exist.");
                }
            }
        }
        

        /// <summary>
        /// Load next level clear's the previous map and returns a new map based on the incremented index.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public Tilemap LoadNextLevel(Tilemap map)
        {
            if (currentLevelIndex < levels.Count - 1)
            {
                currentLevelIndex++;
                Tilemap newMap = CreateMap();
                return newMap;
            }
            else
            {
                GameManager.DisplayVictory();
                return null;
            }
        }

        /// <summary>
        /// Create map method creates a new map and load's it from a text file. Returns the map.
        /// </summary>
        /// <returns></returns>
        public Tilemap CreateMap()
        {
            map = new Tilemap();
            map.LoadFromFile(levels[currentLevelIndex]);
            SpawnPoint = map.FindSpawnPoint(levels[currentLevelIndex]);
            return map;
        }


        /// <summary>
        /// Draw's the map.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
        }
}
}
