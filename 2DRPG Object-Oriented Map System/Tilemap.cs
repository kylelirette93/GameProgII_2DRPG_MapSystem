using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tilemap : Component
    {
        public Tile[,] Tiles { get; set; }
        public int TileWidth { get; private set; } = 32; 
        public int TileHeight { get; private set; } = 32;
        public Tilemap()
        {
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
    }
}