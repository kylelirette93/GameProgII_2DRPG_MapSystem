using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    internal class GameObject
{
        // Not collidable by default.

        private bool isCollidable = false;
        public bool IsCollidable { get { return isCollidable; } set { isCollidable = value; } }

        private Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }
        
        private Texture2D sprite;
        public Texture2D Sprite { get { return sprite; } set { sprite = value; } }
}
}
