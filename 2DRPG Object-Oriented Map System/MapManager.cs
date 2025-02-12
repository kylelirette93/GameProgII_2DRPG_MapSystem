using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Map Manager class is responsible for loading map files based on an index.
    /// </summary>
    public class MapManager
{
        // Properties
        public Vector2 SpawnPoint { get; private set; }

        private List<string> _levelPaths;
        public int CurrentLevelIndex => _currentLevelIndex;
        private int _currentLevelIndex;
        private Tilemap _currentMap;

        public Tilemap CurrentMap => _currentMap;

        /// <summary>
        /// Initializes the map manager, loads map files and sets current level index.
        /// </summary>
        public MapManager()
        {
            _currentLevelIndex = 0;
            _levelPaths = new List<string>();
            LoadMaps();
        }
        /// <summary>
        ///  Loads map files by scanning for files at predefined paths.
        /// </summary>
        public void LoadMaps()
        {
            for (int i = 0; i < 3; i++)
            {
                string levelPath = $"Content/Level{i}.txt";
                if (File.Exists(levelPath))
                {
                    _levelPaths.Add(levelPath);
                }
                else
                {
                    Console.WriteLine($"Warning: Level file {levelPath} does not exist.");
                }
            }
        }
        public void NextMap()
        {
            _currentLevelIndex++;
        }

        /// <summary>
        /// Creates and loads a new map from the current level's file.
        /// Sets the spawn point for the player.
        /// </summary>
        /// <returns></returns>
        public Tilemap CreateMap()
        {
            _currentMap = new Tilemap();
            if (_currentLevelIndex < _levelPaths.Count)
            {
                _currentMap.LoadFromFile(_levelPaths[_currentLevelIndex]);
                SpawnPoint = _currentMap.FindSpawnPoint(_levelPaths[_currentLevelIndex]);
                return _currentMap;
            }
            else
            {
                return null;
            }
        }
        public void SetPlayerStartPosition(GameObject player)
        {
           player.GetComponent<Transform>().Position = SpawnPoint;
        }
        /// <summary>
        /// Draw's the current map to screen using the sprite batch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            _currentMap?.Draw(spriteBatch);
        }
}
}
