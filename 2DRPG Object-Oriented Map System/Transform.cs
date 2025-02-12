using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Transform class is responsible for handling the position, rotation, and scale of a game object.
    /// </summary>
    public class Transform : Component
{
        // Auto-implemented transform.
        public Vector2 Position { get; set; } = new Vector2(200, 200);
        // Currently not using rotation for anything, but I can in the future.
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Updates the transform continuously.
        /// </summary>
        public override void Update()
        {
            // Update the transform.
            base.Update();
        }

        /// <summary>
        /// Translate method is responsible for moving the game object.
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector2 translation)
        {
            Position += translation;
        }

        // Currently not using scale logic.
        /// <summary>
        /// Set Scale method is responsible for setting the scale of the game object.
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            Scale = new Vector2(scale, scale);
        }

        /// <summary>
        /// Increase Scale method is responsible for increasing the scale of the game object.
        /// </summary>
        /// <param name="amount"></param>
        public void IncreaseScale(float amount)
        {
            Scale += new Vector2(amount, amount);
        }

    }
}
