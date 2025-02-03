using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class GameEntity
    {
        protected GameManager gameManager;
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected Rectangle collider;
        protected float movementSpeed;
        protected Vector2 movementDirection;
        public Texture2D Sprite { get { return _sprite; } set { _sprite = value; } }
        protected Texture2D _sprite;
        public Vector2 Position { get { return _position; } set { _position = value;  } }
        protected Vector2 _position;

        public GameEntity(GameManager game, Vector2 initialPosition)
        {
            gameManager = game;
            graphics = game.Graphics;
            movementDirection = Vector2.Zero;
        }

        protected virtual void Update(float deltaTime)
        {
            
        }
        public virtual void DrawCollider(Rectangle collider)
        {
            Texture2D pixel = new Texture2D(gameManager.GraphicsDevice, 1, 1);
            // Set the collider as transparent.
            pixel.SetData(new Color[] { Color.Transparent });
            // Draw the collider.
            gameManager.SpriteBatch.Draw(pixel, collider, Color.White);
        }

        public virtual void Draw(GameTime gameTime)
        {
            // Draw collider around object.
            DrawCollider(collider);
            gameManager.SpriteBatch.Draw(Sprite, Position, Color.White);
        }

    }
}
