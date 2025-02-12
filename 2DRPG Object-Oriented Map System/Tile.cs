using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DRPG_Object_Oriented_Map_System
{
    public class Tile
{
        /// <summary>
        /// Checks if the tile is walkable or not.
        /// </summary>
        public bool IsWalkable { get; set; }

        /// <summary>
        /// Checks if it's an exit tile.
        /// </summary>
        public bool IsExit { get; set; }
        /// <summary>
        /// Texture of the tile.
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Source rectangle of the tile.
        /// </summary>
        public Rectangle SourceRectangle { get; set; }
        
        /// <summary>
        /// Used in the tilemap class to draw a tile.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tilePosition"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 tilePosition)
        {
            spriteBatch.Draw(Texture, tilePosition, SourceRectangle, Color.White);
        }
    }
}
