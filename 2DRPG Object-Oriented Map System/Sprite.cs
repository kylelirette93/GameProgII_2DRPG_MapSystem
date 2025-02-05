using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Sprite : Component
{
        Texture2D _texture;

        public Sprite(Texture2D texture)
        {
            _texture = texture;    
        }
        public void Draw()
        {
            // Responsible for drawing the sprite.
        }

        public override void Update()
        {
            // Should be called by game object.
        }

}
}
