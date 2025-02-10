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
    public class MapManager
{
        public Vector2 SpawnPoint { get; set; }
        private Vector2 _spawnPoint;
        private SpriteBatch _spriteBatch;
        List<string> levels = new List<string>();
        private int currentLevelIndex = 0;

        public MapManager()
        {
            LoadMaps();
        }

        public Tilemap map { get; set; }

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
        public Tilemap CreateMap()
        {
            map = new Tilemap();
            map.LoadFromFile(levels[currentLevelIndex]);
            SpawnPoint = map.FindSpawnPoint(levels[currentLevelIndex]);
            return map;
        }

        public Tilemap LoadNextLevel(Tilemap map)
        {
            map.ClearAllTiles();
            if (currentLevelIndex < levels.Count - 1)
            {
                currentLevelIndex++;
                Tilemap newMap = CreateMap();
                return newMap;
            }
            else
            {
                return null;
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
        }
}
}
