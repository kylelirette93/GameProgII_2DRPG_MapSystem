using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Sprite : Component
{
        Texture2D _texture;
        public Texture2D Texture { get { return _texture; } }
        Vector2 Position { get; set; }
        public Rectangle SpriteBounds { get { return _spriteBounds; } set { _spriteBounds = value; } }
        private Rectangle _spriteBounds;

        public Sprite(Texture2D texture)
        {
            _texture = texture;        
            SpriteBounds = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
        }

        public override void Update()
        {
            // Move the sprite based on transform position.
            Position = GameObject.GetComponent<Transform>().Position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, GameObject.GetComponent<Transform>().Position, null, Color.White);
        }
        
}
}
