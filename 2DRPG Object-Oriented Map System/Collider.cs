using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Collider : Component
{
        
        // For triggering events.
        public bool IsTrigger { get; set; }
        private Rectangle colliderBounds;
        Transform colliderTransform;
        Sprite colliderSprite;

        public Collider(Rectangle bounds)
        {
            colliderBounds = bounds;
            // Set trigger to false by default.
            IsTrigger = false;
        }


        public void SetColliderBounds()
        {
            colliderTransform = GameObject.GetComponent<Transform>();
            colliderSprite = GameObject.GetComponent<Sprite>();

            if (colliderTransform != null && colliderSprite != null)
            {
                // Set the collider bounds based on position from transform and sprite dimensions
                colliderBounds = new Rectangle(
                    (int)colliderTransform.Position.X,
                    (int)colliderTransform.Position.Y,
                    colliderSprite.Texture.Width,
                    colliderSprite.Texture.Height
                );
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            { 
                // Drawing a semi-transparent rectangle to represent the collider bounds
                spriteBatch.Draw(
                    Game1.whitePixel,  // Use a 1x1 white texture for drawing the rectangle
                    colliderBounds,
                    Color.Red * 0.5f // Semi-transparent red for visibility
                );
            }
        }

        public override void Update()
        {
            SetColliderBounds();
        }
}
}
