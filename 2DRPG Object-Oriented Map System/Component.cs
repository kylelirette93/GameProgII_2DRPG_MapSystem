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
    /// <summary>
    /// This class is responsible for enforcing an abstract TakeTurn method that all turn components must implement. 
    /// It also contains a name to differentiate between different turn components.
    /// </summary>
    public abstract class TurnComponent : Component
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public TurnComponent()
        {
            IsActive = true;
        }
        public abstract void TakeTurn();
    }

    /// <summary>
    /// This class is responsible for managing the health of a game object. 
    /// It contains a health and max health property, as well as a method to take damage.
    /// </summary>
    public class HealthComponent : Component
    {
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get {  return Health; } }

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            // Debug.WriteLine(this.GameObject + " took " + damage + " damage. Health: " + Health + "/" + MaxHealth + ".");
            
            SoundManager.PlaySound("hurtSound");
            string animationTextureName = "";
            if (GameObject.Tag == "enemy" || GameObject.Tag == "enemy2")
            {
                animationTextureName = "enemy_hurt";
            }
            else if (GameObject.Tag == "player")
            {
                animationTextureName = "player_hurt";
            }
            bool loop = GameObject.Tag == "enemy" || GameObject.Tag == "enemy2"; // Only enemies loop

            Texture2D animationTexture = AssetManager.GetTexture(animationTextureName);
            AnimationComponent stunnedAnimation = new AnimationComponent(animationTexture, 10, false);
            GameObject.AddComponent(stunnedAnimation);
            stunnedAnimation.PlayAnimation();

            if (Health <= 0)
            {
                GameObject.Destroy();
            }
        }
    }
    /// <summary>
    /// This class is responsible for managing the animation of a game object.
    /// </summary>
    public class AnimationComponent : DrawableComponent
    {
        Texture2D spriteSheet;
        Texture2D originalTexture;

        public int frames = 0;
        public int currentFrame = 0;
        public int frameWidth = 32;
        public int frameHeight = 32;
        public float frameSpeed = 64f; 
        public bool isPlaying = false;
        private float frameAccumulator = 0f;
        public bool isLooping = false;

        // Stopwatch to track time.
        private Stopwatch stopwatch; 

        /// <summary>
        /// This constructor initializes the animation component with a sprite sheet, number of frames, and whether or not the animation should loop.
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="frames"></param>
        /// <param name="isLooping"></param>
        public AnimationComponent(Texture2D spriteSheet, int frames, bool isLooping = false)
        {
            this.spriteSheet = spriteSheet;
            this.frames = frames;
            this.isLooping = isLooping;
            stopwatch = new Stopwatch(); 
        }

        /// <summary>
        /// This method plays the animation, storing the original texture and starting the stopwatch.
        /// </summary>
        public void PlayAnimation()
        {
            if (GameObject?.GetComponent<Sprite>() != null)
            {
                originalTexture = GameObject.GetComponent<Sprite>().Texture; 
                isPlaying = true;
                currentFrame = 0;
                stopwatch.Restart(); 
            }
        }

        /// <summary>
        /// This method stops the animation, setting the texture back to the original texture and stopping the stopwatch.
        /// </summary>
        public void StopAnimation()
        {
            if (GameObject?.GetComponent<Sprite>() != null && originalTexture != null)
            {
                GameObject.GetComponent<Sprite>().Texture = originalTexture;
                isPlaying = false;
                originalTexture = null;
                stopwatch.Stop(); 
            }
        }

        public override void Update()
        {
            if (isPlaying)
            {
                float elapsedMilliseconds = (float)stopwatch.Elapsed.TotalMilliseconds;
                stopwatch.Restart();
                frameAccumulator += elapsedMilliseconds;

                while (frameAccumulator >= frameSpeed)
                {
                    frameAccumulator -= frameSpeed;
                    currentFrame++;

                    if (currentFrame >= frames)
                    {
                        if (isLooping)
                        {
                            currentFrame = 0;
                        }
                        else
                        {
                            StopAnimation();
                            return;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameObject?.GetComponent<Transform>() != null && isPlaying)
            {
                Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); 

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

    public class ItemComponent : Component
    {
        string name;
        string description;

        public ItemComponent(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public void Remove()
        {
            Debug.WriteLine("Used " + name + ".");
            GameObject.RemoveComponent<Component>();
        }
    }

    public class HealingComponent : ItemComponent
    {
        private HealthComponent healthComponent;
        public HealingComponent(string name, string description) : base(name, description)
        {
            if (GameObject == null) return;
            healthComponent = GameObject.GetComponent<HealthComponent>();
        }

        public void Use()
        {
           if (healthComponent != null)
            {
                healthComponent.Health += 10;
                if (healthComponent.Health > healthComponent.MaxHealth)
                {
                    healthComponent.Health = healthComponent.MaxHealth;
                }
                Remove();
            }
        }


    }
}
