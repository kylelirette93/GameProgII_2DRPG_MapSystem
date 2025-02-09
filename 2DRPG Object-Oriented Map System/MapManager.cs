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
                currentLevelIndex = i;
                levels.Add(String.Format("Content/Level{0}.txt", i));
            }
        }
        public Tilemap CreateMap()
        {
            map = new Tilemap();
            map.LoadFromFile(levels[currentLevelIndex]);      
            _spawnPoint = map.FindSpawnPoint(levels[currentLevelIndex]);
            return map;
        }

        public void LoadNextLevel()
        {
            if (currentLevelIndex < levels.Count - 1)
            {
                _spawnPoint = map.FindSpawnPoint(levels[currentLevelIndex + 1]);
                currentLevelIndex++;
                CreateMap();
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
        }
}
}
