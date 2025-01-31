using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class GameEntity : GameObject
    {
        protected GameManager gameManager;
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected Rectangle collider;
        protected float movementSpeed;
        protected Vector2 movementDirection;

        public GameEntity(GameManager game, Vector2 initialPosition)
        {
            gameManager = game;
            graphics = game.Graphics;
            Position = initialPosition;
            movementDirection = Vector2.Zero;
        }

        protected virtual void Update(float deltaTime)
        {
            // Move the entity.
            if (IsCollidable)
            {
                // Draw a collider.
                collider = new Rectangle((int)Position.X, (int)Position.Y, Sprite.Width, Sprite.Height);
            }
        }
        private void DrawCollider(Rectangle collider)
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
