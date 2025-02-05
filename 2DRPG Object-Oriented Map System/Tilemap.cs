using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tilemap : Component
{
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _width;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        private int _height;
        
        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }
        private Tile[,] _tiles;

        public void Draw()
        {
            // Draw the tilemap.
            
        }

        public override void Update()
        {
            // Update the tilemap.
        }
}
}
