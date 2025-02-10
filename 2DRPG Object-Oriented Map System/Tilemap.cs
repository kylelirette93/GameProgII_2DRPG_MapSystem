using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tilemap : Component
    {
        public Tile[,] Tiles { get; set; }
        public int TileWidth { get; private set; } = 32; 
        public int TileHeight { get; private set; } = 32;
        private Dictionary<Char, Tile> tileMappings;
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

        public override void Update()
        {
            //TODO: IMPLEMENT UPDATE.
        }

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