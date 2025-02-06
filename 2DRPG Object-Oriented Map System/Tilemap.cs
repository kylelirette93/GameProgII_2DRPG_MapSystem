using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tilemap : Component
    {
        public Tile[,] Tiles { get; set; }
        public int TileWidth { get; private set; } = 16;
        public int TileHeight { get; private set; } = 16;
        Vector2 position;

        public Tilemap()
        {
            position = new Vector2(0, 0);
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
                    Tile tile = Tiles[x, y];
                    Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
                    tile.Draw(spriteBatch, position);
                }
            }
        }

        public void GenerateProceduralMap(int width, int height)
        {
            TileWidth = 32; // Example tile size
            TileHeight = 32;
            Tiles = new Tile[width, height];

            // Fill the entire map with ground tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Texture2D groundTexture = SpriteManager.GetTexture("ground_tile");

                    Tiles[x, y] = new Tile
                    {
                        IsWalkable = true,
                        Texture = groundTexture,
                        SourceRectangle = new Rectangle(0, 0, TileWidth, TileHeight) // Example source rectangle
                    };
                }
            }

            // Add walls around the edges with specific types
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        // Top-left corner
                        Texture2D wallTexture = SpriteManager.GetTexture("top_west_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == 0 && y == height - 1)
                    {
                        // Bottom-left corner
                        Texture2D wallTexture = SpriteManager.GetTexture("bottom_west_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1 && y == 0)
                    {
                        // Top-right corner
                        Texture2D wallTexture = SpriteManager.GetTexture("top_east_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1 && y == height - 1)
                    {
                        // Bottom-right corner
                        Texture2D wallTexture = SpriteManager.GetTexture("bottom_east_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == 0)
                    {
                        // Left wall
                        Texture2D wallTexture = SpriteManager.GetTexture("west_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (x == width - 1)
                    {
                        // Right wall
                        Texture2D wallTexture = SpriteManager.GetTexture("east_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (y == 0)
                    {
                        // Top wall
                        Texture2D wallTexture = SpriteManager.GetTexture("north_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                    else if (y == height - 1)
                    {
                        // Bottom wall
                        Texture2D wallTexture = SpriteManager.GetTexture("south_wall");
                        Tiles[x, y].Texture = wallTexture;
                        Tiles[x, y].IsWalkable = false;
                    }
                }
            }
        }
    }
}
