using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Tile class is responsible for managing tile properties.
    /// </summary>
    public class Tile
{
        private bool _isWalkable;
        private bool _isExit;
        private Texture2D _texture;
        private Rectangle _sourceRectangle;
        /// <summary>
        /// Checks if the tile is walkable or not.
        /// </summary>
        public bool IsWalkable { get { return _isWalkable; } set { _isWalkable = value; } }

        /// <summary>
        /// Checks if it's an exit tile.
        /// </summary>
        public bool IsExit { get { return _isExit; } set { _isExit = value; } }
        /// <summary>
        /// Texture of the tile.
        /// </summary>
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        /// <summary>
        /// Source rectangle of the tile.
        /// </summary>
        public Rectangle SourceRectangle { get { return _sourceRectangle; } set { _sourceRectangle = value; } }
        
        /// <summary>
        /// Used in the tilemap class to draw a tile.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tilePosition"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 tilePosition)
        {
            spriteBatch.Draw(_texture, tilePosition, _sourceRectangle, Color.White);
        }
    }
}
