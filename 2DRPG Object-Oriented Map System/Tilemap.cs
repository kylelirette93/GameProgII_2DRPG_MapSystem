using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class Tilemap : GameEntity
    {
        // Array of tiles for the map.
        Tile[,] tiles;
        Vector2 mapPosition;
        int mapHeight;
        int mapWidth;
        int tileSize = 16;

        public Tilemap(GameManager game, Vector2 initialPosition) : base(game, initialPosition)
        {
            gameManager = game;
            graphics = game.Graphics;
            mapWidth = graphics.PreferredBackBufferWidth;
            mapHeight = graphics.PreferredBackBufferHeight;
            Position = initialPosition;
            InitializeTiles();
            InitializeMap();
        }

        public void InitializeTiles()
        {
            tiles = new Tile[mapWidth, mapHeight]; // Corrected dimensions
        }

        public void InitializeMap()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector2 tilePosition = new Vector2(
                        Position.X + (x * tileSize),  // Ensuring proper alignment
                        Position.Y + (y * tileSize)
                    );

                    tiles[x, y] = new Tile(gameManager, tilePosition);
                    tiles[x, y].Sprite = gameManager.Content.Load<Texture2D>("ground_tile");
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (tiles[x, y] != null) // Check if tile is not null
                    {
                        tiles[x, y].Draw(spriteBatch);
                    }
                }
            }
        }
    }
}