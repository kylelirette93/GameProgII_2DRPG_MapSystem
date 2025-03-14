using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Sprite class is responsible for rendering a texture to the screen.
    /// </summary>
    public class Sprite : DrawableComponent
{
        private Texture2D _texture;
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { _position = value; } }
        public Rectangle SpriteBounds { get { return _spriteBounds; } set { _spriteBounds = value; } }
        private Rectangle _spriteBounds;
        public Color Color { get { return _color; } set { _color = value; } }
        private Color _color = Color.White;
        

        /// <summary>
        /// Sprite constructor is responsible for setting the texture and sprite bounds.
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;
            _position = Vector2.Zero;
            _spriteBounds = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
        }
        /// <summary>
        /// Update method is responsible for updating the sprite's position.
        /// </summary>
        public override void Update()
        {
            // Update the sprite and it's bounds based on transform position.
            _position = GameObject.GetComponent<Transform>().Position;
            _spriteBounds.Location = _position.ToPoint();
        }
        /// <summary>
        /// Draw method is responsible for rendering the sprite to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the sprite at the object's position.
            spriteBatch.Draw(_texture, _position, null, _color);
        }     
}
}
