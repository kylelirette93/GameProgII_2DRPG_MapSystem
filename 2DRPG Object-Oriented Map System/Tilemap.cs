using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class Tilemap
    {
        // Array of tiles for the map.
        public Tile[][] Tiles { get; private set; }

        public Tilemap(string filePath)
        {
            LoadFromFile(filePath);
        }

        private void LoadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            Tiles = new Tile[lines.Length][];

            for (int y = 0; y < lines.Length; y++)
            {
                Tiles[y] = new Tile[lines[y].Length];
                for (int x = 0; x < lines[y].Length; x++)
                {
                    Tiles[y][x] = CreateTile(lines[y][x]);
                }
            }
        }

        private Tile CreateTile(char symbol)
        {
            return symbol switch
            {
                'G' => new GrassTile(),
                'W' => new WallTile(),
                _ => null
            };
        }
    }
}