using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public abstract class Tile
    {
        public abstract bool IsWalkable { get; }
        public char Symbol { get; set; }

        protected Tile(char symbol) => Symbol = symbol;

    }

    public class GrassTile : Tile
    {
        public override bool IsWalkable => true;
        public GrassTile() : base('G') { }
    }

    public class WallTile : Tile    
    {
        public override bool IsWalkable => false;

        public WallTile() : base('W') { }
    }

}
