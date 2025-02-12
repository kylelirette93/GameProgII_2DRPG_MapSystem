using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Sprite class is responsible for rendering a texture to the screen.
    /// </summary>
    public class Sprite : Component
{
        Texture2D _texture;
        public Texture2D Texture { get { return _texture; } }
        Vector2 Position { get; set; }
        public Rectangle SpriteBounds { get { return _spriteBounds; } set { _spriteBounds = value; } }
        private Rectangle _spriteBounds;

        /// <summary>
        /// Sprite constructor is responsible for setting the texture and sprite bounds.
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;        
            SpriteBounds = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
        }
        /// <summary>
        /// Update method is responsible for updating the sprite's position.
        /// </summary>
        public override void Update()
        {
            // Move the sprite based on transform position.
            Position = GameObject.GetComponent<Transform>().Position;
        }
        /// <summary>
        /// Draw method is responsible for rendering the sprite to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, GameObject.GetComponent<Transform>().Position, null, Color.White);
        }     
}
}
