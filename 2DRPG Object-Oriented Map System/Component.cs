using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Component is an abstract class that all components inherit from. It is used to enforce a common interface for all components.
    /// </summary>
    public abstract class Component
    {
        // Different components inherit from this class.

        // Property for the game object this component is attached to.
        /// <summary>
        /// Game object property for the component.
        /// </summary>
        protected GameObject GameObject { get; private set; }

        /// <summary>
        /// Set Game Object method is responsible for setting the game object for the component.
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        /// <summary>
        /// Update method is responsible for updating the component.
        /// </summary>
        public virtual void Update()
        {
            // This method is overridden by the inheriting class.
        }
    }

    /// <summary>
    /// Drawable component is an abstract class that all drawable components inherit from. 
    /// </summary>
    public abstract class DrawableComponent : Component
    {
        /// <summary>
        /// Draw method for drawable components, I.E Sprite, tilemap, collider, etc.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }

    public abstract class TurnComponent : Component
    {
        public abstract void TakeTurn();
    }

    public class HealthComponent : Component
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            // Give some cool damage feedback.
            FlashRed();
            if (Health <= 0)
            {
                GameObject.Destroy();
            }
        }

        private async void FlashRed()
        {
            // Store original color.
            Sprite spriteComponent = GameObject?.GetComponent<Sprite>();

            if (spriteComponent == null || GameObject == null)
            {
                return; // Exit if GameObject or Sprite is already null.
            }

            Color originalColor = spriteComponent.Color;

            // Flash red.
            spriteComponent.Color = Color.Red;
            AnimationComponent stunnedAnimation = new AnimationComponent(SpriteManager.GetTexture("enemy_stunned"), 10);
            GameObject.AddComponent(stunnedAnimation);
            stunnedAnimation.PlayAnimation();

            // Delay with null check.
            await Task.Delay(100);

            if (GameObject == null || spriteComponent == null && stunnedAnimation == null)
            {
                return; // Exit if GameObject or Sprite is destroyed during the delay.
            }

            // Restore original color.
            stunnedAnimation.StopAnimation();
            spriteComponent.Color = originalColor;
        }
    }
    public class AnimationComponent : DrawableComponent
    {
        Texture2D spriteSheet;
        Texture2D originalTexture;

        public int frames = 0;
        public int currentFrame = 0;
        public int frameWidth = 32;
        public int frameHeight = 32;
        public int frameSpeed = 5; // Milliseconds per frame.
        public bool isPlaying = false;

        private Stopwatch stopwatch; // Stopwatch to track time.

        public AnimationComponent(Texture2D spriteSheet, int frames)
        {
            this.spriteSheet = spriteSheet;
            this.frames = frames;
            stopwatch = new Stopwatch(); // Initialize stopwatch.
        }

        public void PlayAnimation()
        {
            if (GameObject?.GetComponent<Sprite>() != null)
            {
                originalTexture = GameObject.GetComponent<Sprite>().Texture; // Store original.
                isPlaying = true;
                currentFrame = 0;
                stopwatch.Restart(); // Start the stopwatch.
            }
        }

        public void StopAnimation()
        {
            if (GameObject?.GetComponent<Sprite>() != null && originalTexture != null)
            {
                GameObject.GetComponent<Sprite>().Texture = originalTexture; // Restore.
                isPlaying = false;
                originalTexture = null;
                stopwatch.Stop(); // Stop the stopwatch.
            }
        }

        public override void Update() // No GameTime parameter.
        {
            Debug.WriteLine($"Animation isPlaying: {isPlaying}");
            if (isPlaying)
            {
                if (stopwatch.ElapsedMilliseconds >= frameSpeed)
                {
                    stopwatch.Restart(); // Reset stopwatch.
                    currentFrame++;

                    if (currentFrame >= frames)
                    {
                        currentFrame = 0; // Loop.
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameObject?.GetComponent<Transform>() != null && isPlaying)
            {
                Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); // Correct Calculation.

                Debug.WriteLine($"Draw: currentFrame={currentFrame}, frameWidth={frameWidth}, frameHeight={frameHeight}");
                Debug.WriteLine($"Draw: sourceRectangle={sourceRectangle}");

                // Draw at the GameObject's position.
                spriteBatch.Draw(spriteSheet, GameObject.GetComponent<Transform>().Position, sourceRectangle, Color.White);
            }
            else if (GameObject?.GetComponent<Transform>() != null && GameObject?.GetComponent<Sprite>() != null && !isPlaying)
            {
                // Draw the original texture at the GameObject's position.
                spriteBatch.Draw(GameObject.GetComponent<Sprite>().Texture, GameObject.GetComponent<Transform>().Position, Color.White);
            }
        }

    }
}
