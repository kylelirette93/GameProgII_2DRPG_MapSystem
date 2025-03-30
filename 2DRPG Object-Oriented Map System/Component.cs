using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;


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
        protected string Name { get; set; }

        /// <summary>
        /// Set Game Object method is responsible for setting the game object for the component.
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public virtual void Initialize()
        {

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
    /// This class is responsible for managing the health of a game object. 
    /// It contains a health and max health property, as well as a method to take damage.
    /// </summary>
    public class HealthComponent : DrawableComponent
    {
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get { return Health; } }
        
        string animationTextureName;

        private AnimationComponent stunnedAnimation; // Store the AnimationComponent

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public override void Initialize()
        {
            animationTextureName = "";
            if (GameObject.Tag.StartsWith("enemy"))
            {
                animationTextureName = "enemy_hurt";
            }
            else if (GameObject?.Tag == "player")
            {
                animationTextureName = "player_hurt";
            }
            Texture2D animationTexture = AssetManager.GetTexture(animationTextureName);
            stunnedAnimation = new AnimationComponent(animationTexture, 10, false);
            GameObject.AddComponent(stunnedAnimation);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            SoundManager.PlaySound("hurtSound");
            stunnedAnimation.PlayAnimation(); // Reuse the existing AnimationComponent

            if (Health <= 0)
            {
                GameObject.Destroy();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 healthBarPosition = GameObject.GetComponent<Transform>().Position - new Vector2(10, 10);

            float healthPercentage = (float)Health / MaxHealth;
            int filledWidth = (int)(healthPercentage * 50);

            // Draw the background of the health bar (black)
            spriteBatch.Draw(AssetManager.GetTexture("pixel"), new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, 50, 5), Color.Black); // using a single pixel texture and stretching it.

            // Draw the filled portion of the health bar (red or green)
            Color healthColor = healthPercentage > 0.5f ? Color.Green : Color.Red;
            spriteBatch.Draw(AssetManager.GetTexture("pixel"), new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, filledWidth, 5), healthColor); // using a single pixel texture and stretching it.
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

    public class Inventory : DrawableComponent
    {
        private List<ItemComponent> items = new List<ItemComponent>();
        Texture2D[] slotTextures;
        int inventorySlots = 5;
        SpriteFont inventoryFont;
        KeyboardState currentState;
        Vector2[] slotPosition = new Vector2[5]
        {
            new Vector2(840, 50),
            new Vector2(880, 50),
            new Vector2(920, 50),
            new Vector2(960, 50),
            new Vector2(1000, 50)
        };
        Vector2[] labelPosition = new Vector2[5]
        {
            new Vector2(840, 0),
            new Vector2(880, 0),
            new Vector2(920, 0),
            new Vector2(960, 0),
            new Vector2(1000, 0)
        };

        int slotIndex = 0;

        public Inventory()
        {
            slotTextures = new Texture2D[inventorySlots];
            for (int i = 0; i < inventorySlots; i++)
            {
                slotTextures[i] = AssetManager.GetTexture("default_slot");
            }
            inventoryFont = AssetManager.GetFont("font");
        }

        public void AddItem(ItemComponent item)
        {
            if (items.Count < inventorySlots)
            {
                items.Add(item);
                // Get the index of the last item added.
                int itemIndex = items.Count - 1;
                for (int i = 0; i < inventorySlots; i++)
                    if (item.Name == "Healing Potion")
                    {
                        slotTextures[itemIndex] = AssetManager.GetTexture("healing_potion");
                    }
                    else if (item.Name == "Scroll of Fireball")
                    {
                        slotTextures[itemIndex] = AssetManager.GetTexture("scroll_of_fireball");
                    }
                    else if (item.Name == "Scroll of Lightning")
                    {
                        slotTextures[itemIndex] = AssetManager.GetTexture("scroll_of_lightning");
                    }
            }
            else
            {
                Debug.WriteLine("Inventory is full!");
            }
        }

        public void RemoveItem(ItemComponent item)
        {
            if (items.Contains(item))
            {
                int index = items.IndexOf(item);
                items.Remove(item);
                slotTextures[index] = AssetManager.GetTexture("default_slot"); // Update only the used slot

                // Update remaining textures after shifting.
                for (int i = index; i < items.Count; i++)
                {
                    if (items[i] != null)
                    {
                        if (items[i].Name == "Healing Potion")
                        {
                            slotTextures[i] = AssetManager.GetTexture("healing_potion");
                        }
                        else if (items[i].Name == "Scroll of Fireball")
                        {
                            slotTextures[i] = AssetManager.GetTexture("scroll_of_fireball");
                        }
                        else if (items[i].Name == "Scroll of Lightning")
                        {
                            slotTextures[i] = AssetManager.GetTexture("scroll_of_lightning");
                        }
                    }
                }
                if (items.Count < inventorySlots)
                {
                    slotTextures[items.Count] = AssetManager.GetTexture("default_slot");
                }
            }
        }

        public void UseItem(int slotNumber)
        {
            if (slotNumber >= 0 && slotNumber < items.Count && items[slotNumber] != null)
            {
                items[slotNumber].UseItem();
                RemoveItem(items[slotNumber]);
            }
        }

        public override void Update()
        {
            currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.D1))
            {
                UseItem(0);
            }
            else if (currentState.IsKeyDown(Keys.D2))
            {
                UseItem(1);
            }
            else if (currentState.IsKeyDown(Keys.D3))
            {
                UseItem(2);
            }
            else if (currentState.IsKeyDown(Keys.D4))
            {
                UseItem(3);
            }
            else if (currentState.IsKeyDown(Keys.D5))
            {
                UseItem(4);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < inventorySlots; i++)
            {
                spriteBatch.DrawString(inventoryFont, $"[{(i + 1).ToString()}]", labelPosition[i], Color.White);
                spriteBatch.Draw(slotTextures[i], slotPosition[i], Color.White);
            }
        }
    }

    public class ItemComponent : DrawableComponent
    {
        public string Name { get { return name; } }
        string name;
        string description;

        public ItemComponent(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

       public virtual void UseItem()
        {
            // Do something with the item!
        }

        public virtual void PickupItem()
        {

        }
        public void Remove()
        {
            Debug.WriteLine("Used " + name + ".");
            GameObject.RemoveComponent<ItemComponent>();
        }
    }

    
   public class ProjectileComponent : Component
    {
        public Vector2 Direction { get; set; }
        float velocity = 5f;
        GameObject player;
        public string EnemyTag { get; set; }
        public override void Update()
        {
            var enemyObject = ObjectManager.Find(EnemyTag);

            var projectileTransform = GameObject.GetComponent<Transform>();

            if (projectileTransform != null)
            {
                projectileTransform.Position += Direction * velocity;
                Point projectileTile = TilePosition(projectileTransform.Position);

                var playerTransform = ObjectManager.Find("player")?.GetComponent<Transform>();

                if (playerTransform != null && projectileTile == TilePosition(playerTransform.Position))
                {
                    ObjectManager.Find("player")?.GetComponent<HealthComponent>()?.TakeDamage(1);

                    enemyObject = ObjectManager.Find(EnemyTag);
                    enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); // End Turn
                    GameObject?.Destroy();                 
                    return; // Exit the update method since the object is destroyed.
                }

                if (!IsWalkable(projectileTile))
                {
                    enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); // End Turn
                    GameObject?.Destroy();
                    return; // Exit the update method since the object is destroyed.
                }
            }
            else
            {
                enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); // End Turn
                GameObject?.Destroy();
            }
        }

        private Point TilePosition(Vector2 position)
        {
            int tileX = (int)(position.X / 32);
            int tileY = (int)(position.Y / 32);
            return new Point(tileX, tileY);
        }

        private bool IsWalkable(Point tile)
        {
            // Check if in bounds.
            Tilemap tilemap = ObjectManager.Find("tilemap").GetComponent<Tilemap>();
            if (tile.X < 0 || tile.X >= tilemap.MapWidth || tile.Y < 0 || tile.Y >= tilemap.MapHeight)
            {
                return false;
            }
            return ObjectManager.Find("tilemap").GetComponent<Tilemap>().Tiles[tile.X, tile.Y].IsWalkable;
        }
    }

        public class HealingComponent : ItemComponent
    {
        private HealthComponent healthComponent;
        public HealingComponent(string name, string description) : base(name, description)
        {
            
        }

        public override void UseItem()
        {
            healthComponent = ObjectManager.Find("player")?.GetComponent<HealthComponent>();
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

    public class FireballScroll : ItemComponent
    {
        public FireballScroll(string name, string description) : base(name, description)
        {
        }

        public override void UseItem()
        {
            var player = ObjectManager.Find("player");
            var weapon = player.GetComponent<Weapon>();

            if (weapon != null)
            {
                weapon.waitingForDirection = true; // Set waiting flag
                weapon.CreateDirectionArrows();
            }

            Remove();
        }
    }

    public class LightningScroll : ItemComponent
    {
        public LightningScroll(string name, string description) : base(name, description)
        {

        }

        public override void UseItem()
        {
            List<GameObject> targetEnemies = ObjectManager.FindAllObjectsByTag("enemy");
            foreach (var enemy in targetEnemies)
            {
                enemy.GetComponent<HealthComponent>().TakeDamage(2);
            }
        }
    }

    public class PlayerProjectileComponent : Component
    {
        public Vector2 Direction { get; set; }
        public float Velocity { get; set; } = 5f;

        public override void Update()
        {
            if (GameObject == null) return;

            var transform = GameObject.GetComponent<Transform>();
            transform.Position += Direction * Velocity;

            Point tilePosition = TilePosition(transform.Position);

            // Check for collisions with enemies
            List<GameObject> enemies = ObjectManager.FindAllObjectsByTag("enemy");
            foreach (var enemy in enemies)
            {
                if (TilePosition(enemy.GetComponent<Transform>().Position) == tilePosition)
                {
                    enemy.GetComponent<HealthComponent>()?.TakeDamage(3);
                    GameObject?.Destroy();
                    return;
                }
            }

            // Check for collisions with obstacles
            if (!IsWalkable(tilePosition))
            {
                GameObject?.Destroy();
                return;
            }
        }

        private Point TilePosition(Vector2 position)
        {
            int tileX = (int)(position.X / 32);
            int tileY = (int)(position.Y / 32);
            return new Point(tileX, tileY);
        }

        private bool IsWalkable(Point tile)
        {
            Tilemap tilemap = ObjectManager.Find("tilemap").GetComponent<Tilemap>();
            if (tile.X < 0 || tile.X >= tilemap.MapWidth || tile.Y < 0 || tile.Y >= tilemap.MapHeight)
            {
                return false;
            }
            return tilemap.Tiles[tile.X, tile.Y].IsWalkable;
        }
    }

    public class Weapon : Component
    {
        string name;
        public Vector2 chosenDirection = Vector2.Zero;
        private Vector2 selectedDirection = new Vector2(0, -1); // Default upward
        GameObject fireball;
        bool hasFired = false;
        public bool waitingForDirection = false;
        List<(GameObject arrow, Vector2 direction)> directionArrows = new();
        Color selectedColor = Color.Gray;
        Color defaultColor = Color.White;

        public Weapon(string name)
        {
            this.name = name;
        }

        public Vector2 GetDirectionInput()
        {
            KeyboardState currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.W)) return new Vector2(0, -1);
            if (currentState.IsKeyDown(Keys.A)) return new Vector2(-1, 0);
            if (currentState.IsKeyDown(Keys.S)) return new Vector2(0, 1);
            if (currentState.IsKeyDown(Keys.D)) return new Vector2(1, 0);

            return selectedDirection; // Keep previous direction if no new input
        }

        public override void Update()
        {
            if (GameObject == null) return;
            var playerController = GameObject.GetComponent<PlayerController>();

            if (waitingForDirection)
            {
                playerController.IsShooting = true;
                Vector2 input = GetDirectionInput();
                if (input != selectedDirection)
                {
                    selectedDirection = input;
                    UpdateDirectionArrows();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && selectedDirection != Vector2.Zero)
                {
                    chosenDirection = selectedDirection;
                    
                    playerController.IsShooting = false;
                    ShootWeapon();
                    waitingForDirection = false;
                    RemoveDirectionArrows();
                    playerController.EndTurn();
                }
            }
        }

        public void ShootWeapon()
        {
            Vector2 playerPosition = ObjectManager.Find("player").GetComponent<Transform>().Position;
            fireball = GameObjectFactory.CreateFireball(playerPosition);
            if (fireball != null && playerPosition != null) {
                fireball.GetComponent<Transform>().Position = playerPosition;
                PlayerProjectileComponent projectileComponent = fireball.GetComponent<PlayerProjectileComponent>();
                projectileComponent.Direction = chosenDirection;
            } 
            ObjectManager.AddGameObject(fireball);
            hasFired = true;
            ObjectManager.Find("player").GetComponent<PlayerController>().EndTurn();
        }

        public void CreateDirectionArrows()
        {
            Vector2[] choices =
            {
            new Vector2(1, 0),  // Right
            new Vector2(-1, 0), // Left
            new Vector2(0, -1), // Up
            new Vector2(0, 1)   // Down
        };
            
            float[] rotations =
        {
            -MathHelper.PiOver2, // Right: rotate -90 degrees
            MathHelper.PiOver2,  // Left: rotate +90 degrees
            MathHelper.Pi,       // Up: rotate 180 degrees
            0f                   // Down: no rotation
        };




            Vector2 playerPosition = ObjectManager.Find("player").GetComponent<Transform>().Position;

            for (int i = 0; i < choices.Length; i++)
            {
                GameObject arrow = GameObjectFactory.CreateTurnArrow();
                Transform arrowTransform = arrow.GetComponent<Transform>();
                Sprite sprite = arrow.GetComponent<Sprite>();

                // Corrected offset calculation for downward-facing arrows
                Vector2 offset = Vector2.Zero;
                if (choices[i].X == 1) offset = new Vector2(0, 32);  // Right: offset left
                if (choices[i].X == -1) offset = new Vector2(32, 0); // Left: offset right
                if (choices[i].Y == -1) offset = new Vector2(32, 32);  // Up: offset down
                if (choices[i].Y == 1) offset = new Vector2(0, 0);   // Down: no offset

                arrowTransform.Position = playerPosition + (choices[i] * 32f) + offset;
                arrowTransform.Rotation = rotations[i];

                sprite.Color = defaultColor;

                directionArrows.Add((arrow, choices[i]));
                ObjectManager.AddGameObject(arrow);
            }



        }

        public void UpdateDirectionArrows()
        {
            Vector2 playerPosition = ObjectManager.Find("player").GetComponent<Transform>().Position;

            foreach (var (arrow, direction) in directionArrows)
            {
                Vector2 offset = Vector2.Zero;
                if (direction.X == 1) offset = new Vector2(0, 32);  // Right: offset left
                if (direction.X == -1) offset = new Vector2(32, 0); // Left: offset right
                if (direction.Y == -1) offset = new Vector2(32, 32);  // Up: offset down
                if (direction.Y == 1) offset = new Vector2(0, 0);   // Down: no offset

                arrow.GetComponent<Transform>().Position = playerPosition + (direction * 32) + offset;

                Sprite sprite = arrow.GetComponent<Sprite>();
                if (direction == selectedDirection)
                {
                    sprite.Color = selectedColor;
                }
                else
                {
                    sprite.Color = defaultColor;
                }
            }
        }

        public void RemoveDirectionArrows()
        {
            foreach (var (arrow, _) in directionArrows)
            {
                arrow?.Destroy();
            }
            directionArrows.Clear();
        }
    }
}
