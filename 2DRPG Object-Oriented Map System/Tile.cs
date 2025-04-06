using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Threading;


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
        public bool IsWalkable { get { return _isWalkable; } set { _isWalkable = value; } }
        public bool IsExit { get { return _isExit; } set { _isExit = value; } }
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public Rectangle SourceRectangle { get { return _sourceRectangle; } set { _sourceRectangle = value; } }

        public Vector2 Position { get; set; }
        Vector2 originalPosition;
        bool isShaking = false;
        float shakeTimer = 0.0f;
        float shakeSpeed = 0.5f;

        /// <summary>
        /// Used in the tilemap class to draw a tile.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tilePosition"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 tilePosition)
        {
            if (!isShaking)
            {
                Position = tilePosition;
            }
            spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White);
        }

        /*public void Shake()
        {
            if (!isShaking)
            {
                originalPosition = Position;
                isShaking = true;
                shakeTimer = 0.0f;
            }
        }*/

        public void Update()
        {
            if (isShaking)
            {
                shakeTimer += shakeSpeed; 

                if (shakeTimer < 2.0f)
                {
                    Position += new Vector2((float)Math.Sin(shakeTimer * 2) * 2, 0);
                }
                else
                {
                    Position = originalPosition;
                    isShaking = false;
                }
            }
        }
    }
}