using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Collider class is responsible for handling collision detection.
    /// </summary>
    public class Collider : DrawableComponent
{     
        /// <summary>
        /// IsTrigger is a boolean that checks if a collider is a trigger or not.
        /// </summary>
        public bool IsTrigger { get; set; }
        private Rectangle colliderBounds;
        Transform colliderTransform;
        Sprite colliderSprite;

        /// <summary>
        /// Collider constructor is responsible for setting the collider bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public Collider(Rectangle bounds)
        {
            colliderBounds = bounds;
            // Set trigger to false by default.
            IsTrigger = false;
        }

        /// <summary>
        /// Set Collider Bounds method is responsible for setting the collider bounds.
        /// </summary>
        public void SetColliderBounds()
        {
            colliderTransform = GameObject.GetComponent<Transform>();
            colliderSprite = GameObject.GetComponent<Sprite>();

            if (colliderTransform != null && colliderSprite != null)
            {
                // Set the collider bounds based on position from transform and sprite dimensions.
                colliderBounds = new Rectangle(
                    (int)colliderTransform.Position.X,
                    (int)colliderTransform.Position.Y,
                    colliderSprite.Texture.Width,
                    colliderSprite.Texture.Height
                );
            }
        }

        /// <summary>
        /// Draw method is responsible for rendering the collider bounds to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            {
                // Draws a rectangle to represent collider bounds.
                spriteBatch.Draw(
                    GameManager.whitePixel,  // Reuse the white pixel texture to draw collider bounds.
                    colliderBounds,
                   Color.Red * 0.5f // Semi-Transparent, for collider visibility.
                ); 
            }
        }

        /// <summary>
        /// Update method is responsible for updating the collider bounds.
        /// </summary>
        public override void Update()
        {
            // Update the collider.
            SetColliderBounds();
        }
}
}
