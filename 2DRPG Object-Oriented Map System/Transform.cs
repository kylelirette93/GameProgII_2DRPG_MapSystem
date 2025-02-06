using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Transform : Component
{
        // Auto-implemented transform.
        public Vector2 Position { get; set; } = new Vector2(200, 200);
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        public override void Update()
        {
            // Update the transform.
            base.Update();
        }

        public void SetScale(float scale)
        {
            Scale = new Vector2(scale, scale);
        }
        public void Translate(Vector2 translation)
        {
            Position += translation;
        }

        public void IncreaseScale(float amount)
        {
            Scale += new Vector2(amount, amount);
        }

    }
}
