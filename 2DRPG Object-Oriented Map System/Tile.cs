using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class Tile : GameEntity
{
        enum TileType { Floor, Wall, Exit }
        public Tile(GameManager game, Vector2 position) : base (game, position)
        {
            TileType type = TileType.Floor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, Color.White);
        }
    }
}
