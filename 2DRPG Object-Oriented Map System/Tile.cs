using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tile : Component
{
        public int TileWidth
        {
            get { return _tileWidth; }
            set { _tileWidth = value; }
        }
        private int _tileWidth;
        public int TileHeight
        {
            get { return _tileHeight; }
            set { _tileHeight = value; }
        }
        private int _tileHeight;

        public Texture2D TileTexture
        {
            get { return tileTexture; }
            set { tileTexture = value; }
        }
        private Texture2D tileTexture;
    }
}
