using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Map Manager class is responsible for loading map files based on an index.
    /// </summary>
    public class MapManager
{
        public Vector2 SpawnPoint { get; private set; }
        private List<string> _levelPaths;
        private int _currentLevelIndex;
        private Tilemap _currentMap;
        private Vector2 _lastExitTile;

        public int MapHeight { get { return _mapHeight; } }
        private int _mapHeight = 25;

        public int MapWidth { get { return _mapWidth; } }
        private int _mapWidth = 15;

        /// <summary>
        /// Initializes the map manager, loads map files and sets current level index.
        /// </summary>
        public MapManager()
        {          
            // Initialize map manager and list of level paths.
            _currentLevelIndex = 0;
            _levelPaths = new List<string>();
            LoadMaps();
        }
        /// <summary>
        ///  Loads map files by scanning for files at predefined paths.
        /// </summary>
        public void LoadMaps()
        {
            // Add three levels to the list, because I made three levels.
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
        /// <summary>
        /// Next Map method increments the current level index.
        /// </summary>
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
                _lastExitTile = _currentMap.LastExitTile;
            }
            else
            {           
                _currentMap.GenerateProceduralMap(_mapHeight, _mapWidth);
                SpawnPoint = _lastExitTile;
                _lastExitTile = _currentMap.LastExitTile;
            }
            return _currentMap;
        }

        public Vector2 FindEnemySpawn(string name)
        {
            if (_currentMap != null)
            {
                return _currentMap.FindEnemySpawn(name);
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Set Player Start Position method sets the player's position to the spawn point.
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayerStartPosition(GameObject player)
        {
           player.GetComponent<Transform>().Position = SpawnPoint;
        }
}
}
