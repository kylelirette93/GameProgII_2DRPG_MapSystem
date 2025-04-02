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
        public Vector2 SpawnPoint { get; set; }
        private List<string> _levelPaths;

        public int CurrentLevelIndex { get { return _currentLevelIndex; } }
        private int _currentLevelIndex;
        public Tilemap CurrentMap { get { return _currentMap; } set { _currentMap = value; } }
        private Tilemap _currentMap;
        private Vector2 _lastExitTile;

        public int MapHeight { get { return _mapHeight; } }
        private int _mapHeight = 25;

        public int MapWidth { get { return _mapWidth; } }
        private int _mapWidth = 15;
        private Random _random = new Random();

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
            }
            else
            {   
                //TODO: Load Boss level here.
                _currentMap.GenerateProceduralMap(_mapHeight, _mapWidth);
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
        /// This method clear's the current map and sets it to null.
        /// </summary>
        public void Clear()
        {
            if (_currentMap != null && _currentMap.Tiles != null)
            {
                for (int i = 0; i < _currentMap.Tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < _currentMap.Tiles.GetLength(1); j++)
                    {
                        if (_currentMap != null)
                        {
                            // Set each tile to null.
                            _currentMap.Tiles[i, j] = null;
                        }
                    }
                }
            }
            if (_currentMap != null)
            {
                // Dispose of the textures, and set the map to null.
                _currentMap.ClearMap(); 
                _currentMap = null; 
            }
            _currentLevelIndex = 0;
        }


        /// <summary>
        /// Set Player Start Position method sets the player's position to the spawn point.
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayerStartPosition(GameObject player, GameObject previousExit)
        {
           player.GetComponent<Transform>().Position = previousExit.GetComponent<Transform>().Position;
        }
}
}
