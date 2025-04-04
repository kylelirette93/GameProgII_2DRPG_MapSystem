using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// DisplayIcon class is responsible for displaying the icon on the screen. This is used for the boss fight.
    /// </summary>
    public class DisplayIcon : DrawableComponent
{

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        int timer = 0;

        public DisplayIcon(Vector2 position)
        {
            Position = position;
        }

        public void SetIcon(Texture2D texture)
        {
            Texture = texture;
        }

        public void RemoveIcon(Texture2D texture)
        {
            if (Texture == texture)
            {
                Texture = null;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }

        public override void Update()
        {
           Position = new Vector2(ObjectManager.Find("Boss").GetComponent<Transform>().Position.X, ObjectManager.Find("Boss").GetComponent<Transform>().Position.Y - 32);
            timer++;
            if (timer > 100)
            {
                timer = 0;
                Texture = null;
            }
        }
}
}
