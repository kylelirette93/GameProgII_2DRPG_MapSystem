using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// State that stores the current game mode, to tell the map manager what to load.
    /// </summary>
    public enum GameMode
    {
        Adventure,
        Endless
    }

    public class MapManager
    {
        public Vector2 SpawnPoint { get; set; }
        public List<string> LevelPaths { get { return _levelPaths; } set { _levelPaths = value; } }
        private List<string> _levelPaths;
        public int AdventureLevelIndex { get { return _adventureLevelIndex; } set { _adventureLevelIndex = value; } }
        private int _adventureLevelIndex;
        public int CurrentLevel { get { return currentLevel; } }
        int currentLevel;
        public Tilemap CurrentMap { get { return _currentMap; } set { _currentMap = value; } }
        private Tilemap _currentMap;
        private Vector2 _lastExitTile;
        public int MapHeight { get { return _mapHeight; } }
        private int _mapHeight = 25;
        public int MapWidth { get { return _mapWidth; } }
        private int _mapWidth = 15;

        private Random _random = new Random();
        public GameMode CurrentGameMode { get; set; }

        public MapManager(GameMode gameMode)
        {
            currentLevel = 1;
            _levelPaths = new List<string>();
            LoadStoryMaps();
            _adventureLevelIndex = 0;
            CurrentGameMode = gameMode;
        }

        private void LoadStoryMaps()
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
            if (CurrentGameMode == GameMode.Adventure)
            {
                _adventureLevelIndex++;
            }
        }

        public Tilemap CreateMap()
        {
            _currentMap = new Tilemap();
            if (CurrentGameMode == GameMode.Adventure)
            {
                if (_adventureLevelIndex < _levelPaths.Count)
                {
                    _currentMap.LoadFromFile(_levelPaths[_adventureLevelIndex]);
                }
                else
                {
                    string bossLevel = $"Content/boss.txt";
                    _currentMap.LoadFromFile(bossLevel);
                }
            }
            else
            {
                _currentMap.GenerateProceduralMap(_mapHeight, _mapWidth);
            }
            return _currentMap;
        }
    


        public Vector2 FindEnemySpawn(string name)
        {
            if (_currentMap != null)
            {
                return _currentMap.FindSpawn(name);
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
            _adventureLevelIndex = 0;
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
