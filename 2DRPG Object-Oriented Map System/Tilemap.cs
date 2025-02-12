using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Tilemap Class is responsible for creating and drawing a map. Either procedurally or from a file.
    /// </summary>
    public class Tilemap : Component
    {
        /// <summary>
        /// List of tiles that make up a map.
        /// </summary>
        public Tile[,] Tiles { get; set; }
        /// <summary>
        /// Tile width.
        /// </summary>
        public int TileWidth { get; private set; } = 32;
        /// <summary>
        /// Tile height.
        /// </summary>
        public int TileHeight { get; private set; } = 32;

        private Dictionary<Char, Tile> tileMappings;

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
                { 'P', new Tile { IsWalkable = true, Texture = SpriteManager.GetTexture("ground_tile"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { 'X', new Tile { IsExit = true, Texture = SpriteManager.GetTexture("exit_tile"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '1', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("top_east_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '2', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("top_west_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '3', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("bottom_east_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } },
                { '4', new Tile { IsWalkable = false, Texture = SpriteManager.GetTexture("bottom_west_wall"), SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) } }
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
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (Tiles[x, y] != null)
                    {
                        Tile tile = Tiles[x, y];
                        Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
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
                        return new Vector2(x * TileWidth, y * TileHeight);
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
            Tiles = new Tile[width, height];

            // Initialize all tiles to ground
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[x, y] = new Tile
                    {
                        IsWalkable = true,
                        Texture = SpriteManager.GetTexture("ground_tile"),
                        SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight)
                    };
                }
            }

            // Create the outer walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 && y == 0) // Top-left
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("top_west_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1 && y == 0) // Top-right
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("top_east_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == 0 && y == height - 1) // Bottom-left
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("bottom_west_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1 && y == height - 1) // Bottom-right
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("bottom_east_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == 0) // West wall
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("west_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1) // East wall
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("east_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (y == 0) // North wall
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("north_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (y == height - 1) // South wall
                    {
                        Tiles[x, y].Texture = SpriteManager.GetTexture("south_wall");
                        Tiles[x, y].IsWalkable = false;
                    }
                }
            }
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
            Tiles = new Tile[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = lines[y][x];
                    Tiles[x, y] = CreateTileFromSymbol(symbol);
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