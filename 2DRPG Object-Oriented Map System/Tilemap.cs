using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public Tile[,] Tiles { get; set; }
        private int _tileWidth = 32;
        private int _tileHeight = 32;
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public int TileWidth { get { return _tileWidth; } }
        public int TileHeight { get { return _tileHeight; } }
        public Vector2 LastExitTile { get; set; }
        private Dictionary<char, Tile> tileMappings;
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
            tileMappings = new Dictionary<char, Tile>
            {
                { 'G', CreateTile("ground_tile", true) },
                { 'N', CreateTile("north_wall", false) },
                { 'S', CreateTile("south_wall", false) },
                { 'E', CreateTile("east_wall", false) },
                { 'W', CreateTile("west_wall", false) },
                { 'P', CreateTile("spawn_tile", true) },
                { 'O', CreateTile("obstacle_tile", false) },
                { '1', CreateTile("top_east_wall", false) },
                { '2', CreateTile("top_west_wall", false) },
                { '3', CreateTile("bottom_east_wall", false) },
                { '4', CreateTile("bottom_west_wall", false) },
            };
        }

        private Tile CreateTile(string textureName, bool isWalkable, bool isExit = false)
        {
            return new Tile
            {
                IsWalkable = isWalkable,
                IsExit = isExit,
                Texture = AssetManager.GetTexture(textureName),
                SourceRectangle = new Rectangle(0, 0, _tileWidth, _tileHeight)
            };
        }
        /// <summary>
        /// Tilemap class's Update method is not implemented, no need yet.
        /// </summary>
        public override void Update() 
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Tiles[x, y]?.Update();
                }
            }
        }

        /// <summary>
        /// Draw method is responsible for drawing the map to the screen based on the tiles height and width.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Tiles == null) return;
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Tiles[x, y]?.Draw(spriteBatch, new Vector2(x * _tileWidth, y * _tileHeight));
                }
            }
        }

        /// <summary>
        /// Generate's a procedural map based on the width and height and tile rules.
        /// </summary>
        /// <param name="width">Width of map being generated.</param>
        /// <param name="height">Height of map being generated.</param>
        public void GenerateProceduralMap(int width, int height)
        {
            Tiles = new Tile[width, height];
            MapWidth = width;
            MapHeight = height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Set all tiles to ground initially.
                    Tiles[x, y] = CreateTile("ground_tile", true);
                }
            }

            for (int x = 0; x < width; x++)
            {
                Tiles[x, 0] = CreateTile(x == 0 ? "top_west_wall" : (x == width - 1 ? "top_east_wall" : "north_wall"), false);
                Tiles[x, height - 1] = CreateTile(x == 0 ? "bottom_west_wall" : (x == width - 1 ? "bottom_east_wall" : "south_wall"), false);
            }

            for (int y = 1; y < height - 1; y++)
            {
                Tiles[0, y] = CreateTile("west_wall", false);
                Tiles[width - 1, y] = CreateTile("east_wall", false);
            }

            float baseObstacleChance = 0.25f;
            Vector2 center = new Vector2(width / 2f, height / 2f);
            float maxDistance = Vector2.Distance(center, new Vector2(0, 0));

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    Vector2 tilePos = new Vector2(x, y);
                    float distance = Vector2.Distance(center, tilePos);
                    float obstacleChance = baseObstacleChance * (1 - (distance / maxDistance));

                    if (random.NextDouble() < obstacleChance && (x != (int)LastExitTile.X || y != (int)LastExitTile.Y))
                    {
                        Tiles[x, y] = CreateTile("obstacle_tile", false);
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Tile[,] tempTiles = (Tile[,])Tiles.Clone();
                for (int x = 1; x < width - 2; x++)
                {
                    for (int y = 1; y < height - 2; y++)
                    {
                        int neighbourCount = CountObstacleNeighbors(x, y);
                        tempTiles[x, y] = neighbourCount > 4 ? CreateTile("obstacle_tile", false) : (neighbourCount < 2 ? CreateTile("ground_tile", true) : tempTiles[x, y]);
                    }
                }
                Tiles = tempTiles;
            }
        }

        private int CountObstacleNeighbors(int x, int y)
        {
            int obstacleCount = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx != 0 || dy != 0) // Exclude the center tile.
                    {
                        int neighborX = x + dx;
                        int neighborY = y + dy;

                        // Check if the neighbor is within the tile grid bounds.
                        if (neighborX >= 0 && neighborX < Tiles.GetLength(0) &&
                            neighborY >= 0 && neighborY < Tiles.GetLength(1))
                        {
                            if (!Tiles[neighborX, neighborY].IsWalkable)
                            {
                                obstacleCount++;
                            }
                        }
                    }
                }
            }

            return obstacleCount;
        }

        /// <summary>
        /// Replace's a tile at a specific x and y position with a new tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tile"></param>
        public void SetTile(int x, int y, Tile tile)
        {
            if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight) return;
            Tiles[x, y] = tile;
        }

        /// <summary>
        /// Clear Path to Exit method is responsible for clearing a path from the player to the exit.
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <param name="exitPosition"></param>
        public void ClearPathToExit(Point playerPosition, Point exitPosition)
        {
            Point current = playerPosition;
            Point target = exitPosition;

            SetTile(current.X, current.Y, CreateTile("ground_tile", true));

            while (current != target)
            {
                current.X += Math.Sign(target.X - current.X);
                current.Y += Math.Sign(target.Y - current.Y);

                if (current.X >= 0 && current.X < MapWidth && current.Y >= 0 && current.Y < MapHeight)
                {
                    SetTile(current.X, current.Y, CreateTile("ground_tile", true));
                }
                else break;
            }
        }

        private Vector2 FindValidSpawn(Vector2 playerPosition, float minDistance)
        {
            Vector2 position;
            bool isValid = false; // Add a boolean to control the loop.

            do
            {
                position = new Vector2(random.Next(2, MapWidth - 2) * _tileWidth, random.Next(2, MapHeight - 2) * _tileHeight);

                // Check if the tile at the generated position is walkable.
                int tileX = (int)(position.X / _tileWidth);
                int tileY = (int)(position.Y / _tileHeight);

                if (tileX >= 0 && tileX < Tiles.GetLength(0) && tileY >= 0 && tileY < Tiles.GetLength(1))
                {
                    if (Tiles[tileX, tileY].IsWalkable && Vector2.Distance(position, playerPosition) >= minDistance)
                    {
                        isValid = true;
                    }
                }
            } while (!isValid); // Loop until a valid position is found.

            return position;
        }

        public Vector2 FindExitSpawn(Vector2 playerPosition, float minDistance) => FindValidSpawn(playerPosition, minDistance);
        public Vector2 FindItemSpawn(Vector2 playerPosition, float minDistance) => FindValidSpawn(playerPosition, minDistance);

        public Vector2 FindSpawn(string name)
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (Tiles[x, y].IsWalkable) return new Vector2(x * _tileWidth, y * _tileHeight);
                }
            }
            return Vector2.Zero;
        }
        /// <summary>
        /// Load's a map from a file based on the file path.
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            MapWidth = lines[0].Length;
            MapHeight = lines.Length;
            Tiles = new Tile[MapWidth, MapHeight];

            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    Tiles[x, y] = CreateTileFromSymbol(lines[y][x]);
                }
            }
        }
        /// <summary>
        /// Create's a tile based on the symbol from the file.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Tile CreateTileFromSymbol(char symbol)
        {
            if (tileMappings.ContainsKey(symbol))
            {
                return tileMappings[symbol];
            }
            else
            {
                throw new Exception($"Unknown tile symbol: {symbol}");
            }
        }

        public void ClearMap()
        {
            if (Tiles == null) return;
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Tiles[x, y]?.Texture?.Dispose();
                    Tiles[x, y] = null;
                }
            }
        }

        /*public void Shake()
        {
            // Shake the tilemap!
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (Tiles[x, y] != null)
                    {
                        Tiles[x, y].Shake();
                    }
                }
            }
        }
        */
    }
}