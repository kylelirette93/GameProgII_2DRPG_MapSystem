using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tile
{
        public bool IsWalkable { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        
        // Add collider to non walkable tiles.
        public Collider Collider { get; set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 tilePosition)
        {
            spriteBatch.Draw(Texture, tilePosition, SourceRectangle, Color.White);
        }
    }
}
