using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Tilemap Class is responsible for creating and drawing a map. Either procedurally or from a file.
    /// </summary>
    public class Tilemap : DrawableComponent
    {
        /// <summary>
        /// List of tiles that make up a map.
        /// </summary>
        public Tile[,] Tiles { get { return _tiles; } set { _tiles = value; } }
        private Tile[,] _tiles;

        private int _tileWidth = 32;
        private int _tileHeight = 32;

        /// <summary>
        /// Tile width.
        /// </summary>
        public int TileWidth { get { return _tileWidth; } }
        /// <summary>
        /// Tile height.
        /// </summary>
        public int TileHeight { get { return _tileHeight; } }
        private Vector2 lastExitTile;
        public Vector2 LastExitTile { get { return _lastExitTile; } set { _lastExitTile = value; } }
        private Vector2 _lastExitTile;

        private Dictionary<Char, Tile> tileMappings;
        int exitX;
        int exitY;
        Random random = new Random();

        /// <summary>
        /// Tilemap constructor initializes the tile mappings to a dictionary.
        /// </summary>
        public Tilemap()
        {
            InitializeTileMappings();
        }

        private void InitializeTileMappings()
        {
            // Initialize dictionary with tile mappings based on symbols.
            tileMappings = new Dictionary<char, Tile>
            {
                { 'G', new Tile { IsWalkable = true, Texture = SpriteManager.GetTexture("ground_tile"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'N', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("north_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'S', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("south_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'E', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("east_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'W', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("west_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'P', new Tile { IsWalkable = true, Texture = SpriteManager.GetTexture("spawn_tile"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'X', new Tile { IsExit = true, Texture = SpriteManager.GetTexture("exit_tile"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '1', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("top_east_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '2', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("top_west_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '3', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("bottom_east_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '4', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("bottom_west_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
            };
        }

        /// <summary>
        /// Tilemap class's Update method is not implemented, no need yet.
        /// </summary>
        public override void Update()
        {
            //TODO: IMPLEMENT UPDATE.
        }

        /// <summary>
        /// Draw method is responsible for drawing the map to the screen based on the tiles height and width.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x <_tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y] != null)
                    {
                        Tile tile = _tiles[x, y];
                        Vector2 position = new Vector2(x * _tileWidth, y * _tileHeight);
                        tile.Draw(spriteBatch, position);
                    }
                }
            }
        }
       

        /// <summary>
        /// Iterates through the map file to find the spawn point.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Vector2 FindSpawnPoint(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int width = lines[0].Length;
            int height = lines.Length;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = lines[y][x];
                    if (symbol == 'P')
                    {
                        return new Vector2(x * _tileWidth, y * _tileHeight);
                    }
                }
            }
            throw new Exception("Spawn point 'P' not found in map file.");
        }

        /// <summary>
        /// Generate's a procedural map based on the width and height and tile rules.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GenerateProceduralMap(int width, int height)
        {
            _tiles = new Tile[width, height];
            Random random = new Random();          

            // Initialize all tiles to ground
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _tiles[x, y] = new Tile
                    {
                        IsWalkable = true,
                        Texture = SpriteManager.GetTexture("ground_tile"),
                        SourceRectangle = new Rectangle(0, 0, _tileWidth, _tileHeight)
                    };
                }
            }

            // Create the outer walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 && y == 0) // Top-left
                        SetWall(x, y, "top_west_wall");
                    else if (x == width - 1 && y == 0) // Top-right
                        SetWall(x, y, "top_east_wall");
                    else if (x == 0 && y == height - 1) // Bottom-left
                        SetWall(x, y, "bottom_west_wall");
                    else if (x == width - 1 && y == height - 1) // Bottom-right
                        SetWall(x, y, "bottom_east_wall");
                    else if (x == 0) // Left wall
                        SetWall(x, y, "west_wall");
                    else if (x == width - 1) // Right wall
                        SetWall(x, y, "east_wall");
                    else if (y == 0) // Top wall
                        SetWall(x, y, "north_wall");
                    else if (y == height - 1) // Bottom wall
                        SetWall(x, y, "south_wall");
                }
            }
            exitX = random.Next(4, width - 4);
            exitY = random.Next(4, height - 4);
            _tiles[exitX, exitY].Texture = SpriteManager.GetTexture("exit_tile");
            _tiles[exitX, exitY].IsExit = true;
            LastExitTile = new Vector2(exitX * _tileWidth, exitY * _tileHeight);
            Debug.WriteLine(LastExitTile);

            // Randomly seed obstacles.
            float obstacleChance = 0.35f;
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (random.NextDouble() < obstacleChance && (x != (int)LastExitTile.X || y != (int)LastExitTile.Y))
                    {                     
                        _tiles[x, y].Texture = SpriteManager.GetTexture("obstacle_tile");
                        _tiles[x, y].IsWalkable = false;
                    }
                }
            }


            // Apply cellular automata rules for smooth obstacles.
            for (int i = 0; i < 3; i++)
            {
                Tile[,] tempTiles = (Tile[,])_tiles.Clone();

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        int neighbourCount = CountObstacleNeighbors(x, y);

                        if (neighbourCount > 4)
                            SetObstacle(tempTiles, x, y);
                        else if (neighbourCount < 2)
                            SetGround(tempTiles, x, y);
                    }
                }
                _tiles = tempTiles;
            }
            

        }
        // Helper to set a wall tile
        private void SetWall(int x, int y, string texture)
        {
            _tiles[x, y].Texture = SpriteManager.GetTexture(texture);
            _tiles[x, y].IsWalkable = false;
        }

        // Helper to set an obstacle tile
        private void SetObstacle(Tile[,] map, int x, int y)
        {
            map[x, y].Texture = SpriteManager.GetTexture("obstacle_tile");
            map[x, y].IsWalkable = false;
        }

        private void SetGround(Tile[,] map, int x, int y)
        {
            map[x, y].Texture = SpriteManager.GetTexture("ground_tile");
            map[x, y].IsWalkable = true;
        }

        private int CountObstacleNeighbors(int x, int y)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // Skip self
                    if (!_tiles[x + dx, y + dy].IsWalkable) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Load's a map from a file based on the file path.
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int width = lines[0].Length;
            int height = lines.Length;
            _tiles = new Tile[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = lines[y][x];
                    // Store the last exit tile.
                    if (symbol == 'X')
                    {
                        LastExitTile = new Vector2(x * TileWidth, y * TileHeight);
                    }
                    _tiles[x, y] = CreateTileFromSymbol(symbol);
                }
            }
        }
        public Vector2 FindEnemySpawn(string name)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y].IsWalkable)
                    {
                        return new Vector2(x * _tileWidth, y * _tileHeight);
                    }
                }              
            }
            return Vector2.Zero;
        }
        /// <summary>
        /// Create's a tile based on the symbol from the file.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Tile CreateTileFromSymbol(char symbol)
        {
            if (tileMappings.TryGetValue(symbol, out Tile tile))
            {
                return tile;
            }
            else
            {
                throw new Exception($"Unknown tile symbol: {symbol}");
            }
        }
    
    }
}