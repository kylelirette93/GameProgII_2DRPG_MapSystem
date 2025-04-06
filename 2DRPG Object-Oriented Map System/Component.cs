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

        public string animationTextureName;

        private AnimationComponent stunnedAnimation; 

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public override void Initialize()
        {
            animationTextureName = "";
            EnemyType enemyType = GameObject?.GetComponent<EnemyType>();

            if (enemyType != null)
            {
                // Check's which enemy type we should animation. 
                if (enemyType.Type == "ranged")
                {
                    animationTextureName = "ranged_enemy_hurt";
                }
                else if (enemyType.Type == "melee")
                {
                    animationTextureName = "enemy_hurt";
                }
                else if (enemyType.Type == "ghost")                 
                {
                    animationTextureName = "ghost_enemy_hurt";
                }
                else if (enemyType.Type == "boss")
                {
                    animationTextureName = "boss_hurt";
                }
            }
            else if (GameObject?.Tag == "player")
            {
                animationTextureName = "player_hurt";
            }
            Texture2D animationTexture = AssetManager.GetTexture(animationTextureName);
            stunnedAnimation = new AnimationComponent(animationTexture, 10, false);
            GameObject.AddComponent(stunnedAnimation);
        }

        /// <summary>
        /// Responsible for handling damage of a game object, and destroying if dead.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            Health -= damage;
            SoundManager.PlaySound("hurtSound");
            stunnedAnimation.PlayAnimation(); 

            if (Health <= 0)
            {
                if (GameObject?.Tag == "player")
                {
                    //Debug.WriteLine("Destroying:" + GameObject.Tag);
                    GameObject?.Destroy();
                    //Debug.WriteLine("Game objects still in list: " + ObjectManager.GameObjects.Count);
                    GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
                }
                else 
                {

                    var enemyAI = GameObject?.GetComponent<BaseEnemyAI>();
                    if (enemyAI is BossEnemyAI)
                    {
                        TurnManager.Instance.RemoveTurnTaker(enemyAI);
                        GameManager.Instance.ChangeState(GameManager.GameState.GameWin);
                    }
                    TurnManager.Instance.RemoveTurnTaker(enemyAI);
                    GameObject.Destroy();

                }
            }
        }

        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Position a health bar just above a game object's head.
            Vector2 healthBarPosition = GameObject.GetComponent<Transform>().Position - new Vector2(10, 10);

            // Calculate the percentage of health remaining, and the width of the health bar.
            float healthPercentage = (float)Health / MaxHealth;
            int filledWidth = (int)(healthPercentage * 50);

            // Draw the background and fill portion of the health bar, stretching the filled portion based on health percentage.
            spriteBatch.Draw(AssetManager.GetTexture("pixel"), new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, 50, 5), Color.Black);
            Color healthColor = healthPercentage > 0.5f ? Color.Green : Color.Red;
            spriteBatch.Draw(AssetManager.GetTexture("pixel"), new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, filledWidth, 5), healthColor); 
        }
    }

    /// <summary>
    /// Component responsible for managing the type of an enemy. For animation purposes.
    /// </summary>
    public class EnemyType : Component
    {
        public string Type { get; set; }
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
        public float frameSpeed = 128f;
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
                GameObject.GetComponent<Sprite>().IsVisible = false;
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
                GameObject.GetComponent<Sprite>().IsVisible = true;
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
                spriteBatch.Draw(spriteSheet, GameObject.GetComponent<Transform>().Position, sourceRectangle, Color.White);
            }
            // No else condition needed here, as the sprite will be invisible.
        }
    }

    /// <summary>
    /// Inventory Component is responsible for managing the player's inventory. Drawing the inventory slots and items and using items.
    /// </summary>
    public class Inventory : DrawableComponent
    {
        private List<ItemComponent> items = new List<ItemComponent>();
        private ItemComponent itemToRemove = null;
        bool itemUsed = false;
        Texture2D[] slotTextures;
        int inventorySlots = 5;
        SpriteFont inventoryFont;
        KeyboardState currentState;
        // Position of each inventory slot.
        Vector2[] slotPosition = new Vector2[5]
        {
            new Vector2(840, 50),
            new Vector2(880, 50),
            new Vector2(920, 50),
            new Vector2(960, 50),
            new Vector2(1000, 50)
        };
        // Number key positions above slots.
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
                // Fills the slots with default textures.
                slotTextures[i] = AssetManager.GetTexture("default_slot");
            }
            inventoryFont = AssetManager.GetFont("font");
        }

        public void AddItem(ItemComponent item)
        {
            if (items.Count < inventorySlots)
            {
                items.Add(item);

                // Get the index of the last item added so the correct slot is updated.
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
                else if (item.Name == "Scroll of Force")
                {
                    slotTextures[itemIndex] = AssetManager.GetTexture("scroll_of_force");
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
                itemToRemove = item;
            }
        }

        /// <summary>
        /// This method is responsible for calling use item method of the item, and removing it from the inventory.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void UseItem(int slotNumber)
        {
            if (GameObject.GetComponent<PlayerController>() != null && !GameObject.GetComponent<PlayerController>().IsTurn)
            {
                return; 
            }
            if (slotNumber >= 0 && slotNumber < items.Count && items[slotNumber] != null && !itemUsed)
            {
                items[slotNumber].UseItem();
                itemToRemove = items[slotNumber];
                itemUsed = true;
            }
        }

        public override void Update()
        {
            KeyboardState previousState = currentState;
            currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.D1) && previousState.IsKeyUp(Keys.D1)) UseItem(0);
            else if (currentState.IsKeyDown(Keys.D2) && previousState.IsKeyUp(Keys.D2)) UseItem(1);
            else if (currentState.IsKeyDown(Keys.D3) && previousState.IsKeyUp(Keys.D3)) UseItem(2);
            else if (currentState.IsKeyDown(Keys.D4) && previousState.IsKeyUp(Keys.D4)) UseItem(3);
            else if (currentState.IsKeyDown(Keys.D5) && previousState.IsKeyUp(Keys.D5)) UseItem(4);

            if (itemToRemove != null)
            {
                int index = items.IndexOf(itemToRemove);
                items.Remove(itemToRemove);
                itemToRemove = null;

                int originalItemCount = items.Count + 1;
                for (int i = index; i < originalItemCount; i++)
                {
                    if (i < items.Count)
                    {
                        slotTextures[i] = slotTextures[i + 1];
                    }
                    else
                    {
                        slotTextures[i] = AssetManager.GetTexture("default_slot");
                    }
                }

                for (int i = items.Count; i < inventorySlots; i++)
                {
                    slotTextures[i] = AssetManager.GetTexture("default_slot");
                }
                itemUsed = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < inventorySlots; i++)
            {
                spriteBatch.DrawString(inventoryFont, $"[{(i + 1).ToString()}]", labelPosition[i], Color.White);
                if (i < items.Count && items[i] != null)
                {
                    if (items[i].Name == "Healing Potion")
                    {
                        spriteBatch.Draw(AssetManager.GetTexture("healing_potion"), slotPosition[i], Color.White);
                    }
                    else if (items[i].Name == "Scroll of Fireball")
                    {
                        spriteBatch.Draw(AssetManager.GetTexture("scroll_of_fireball"), slotPosition[i], Color.White);
                    }
                    else if (items[i].Name == "Scroll of Lightning")
                    {
                        spriteBatch.Draw(AssetManager.GetTexture("scroll_of_lightning"), slotPosition[i], Color.White);
                    }
                    else if (items[i].Name == "Scroll of Force")
                    {
                        spriteBatch.Draw(AssetManager.GetTexture("scroll_of_force"), slotPosition[i], Color.White);
                    }
                }
                else
                {
                    spriteBatch.Draw(AssetManager.GetTexture("default_slot"), slotPosition[i], Color.White);
                }
            }
        }
    }

    /// <summary>
    /// This class is responsible for managing an inventory item.
    /// </summary>
    public abstract class ItemComponent : DrawableComponent
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

        public void Remove()
        {
            //Debug.WriteLine("Used " + name + ".");
            GameObject.RemoveComponent<ItemComponent>();
        }
    }

    /// <summary>
    /// This class is responsible for managing projectiles fired by the enemy.
    /// </summary>
   public class ProjectileComponent : Component
    {
        /// <summary>
        /// Get's and sets the direction of the projectile.
        /// </summary>
        public Vector2 Direction { get; set; }
        float velocity = 5f;
        GameObject player;

        /// <summary>
        /// Necessary for accessing the enemy object, to end the turn after use.
        /// </summary>
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

                // If the projectile hit the player, deal damage and end the turn.
                if (playerTransform != null && projectileTile == TilePosition(playerTransform.Position))
                {
                    ObjectManager.Find("player")?.GetComponent<HealthComponent>()?.TakeDamage(1);

                    enemyObject = ObjectManager.Find(EnemyTag);
                    enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); 
                    GameObject?.Destroy();                 
                    return; 
                }

                // If projectile hit a wall, destroy it.
                if (!IsWalkable(projectileTile))
                {
                    enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); 
                    GameObject?.Destroy();
                    return; 
                }
            }
            // If the projectile is null, destroy it.
            else
            {
                enemyObject?.GetComponent<RangedEnemyAI>()?.EndTurn(); 
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

    /// <summary>
    /// This class is responsible for managing a potion item.
    /// </summary>
        public class HealingComponent : ItemComponent
    {
        private HealthComponent healthComponent;
        public HealingComponent(string name, string description) : base(name, description) { }
       
        public override void UseItem()
        {
           healthComponent = ObjectManager.Find("player")?.GetComponent<HealthComponent>();
           var playerController = ObjectManager.Find("player")?.GetComponent<PlayerController>();
            if (healthComponent != null)
            {
                healthComponent.Health += 10;
                SoundManager.PlaySound("heal");
                if (healthComponent.Health > healthComponent.MaxHealth)
                {
                    healthComponent.Health = healthComponent.MaxHealth;
                }
                playerController.EndTurn();
                Remove();
            }
        }
    }

    /// <summary>
    /// This class is responsible for managing the fireball scroll.
    /// </summary>
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
                // Set the flag and create arrow's for directions.
                weapon.waitingForDirection = true; 
                weapon.CreateDirectionArrows();
            }
            Remove();
        }
    }

    /// <summary>
    /// This class is responsible for managing the lightning scroll.
    /// </summary>
    public class LightningScroll : ItemComponent
    {
        public LightningScroll(string name, string description) : base(name, description)
        {
        }

        public override void UseItem()
        {
            // Deals damage to each enemy on the map.
            var boss = ObjectManager.Find("Boss");
            var playerController = ObjectManager.Find("player").GetComponent<PlayerController>();
            if (boss != null)
            {
                boss.GetComponent<HealthComponent>()?.TakeDamage(2);
                GameObject lightningStrike = GameObjectFactory.CreateLightningStrike();
                ObjectManager.AddGameObject(lightningStrike);
                lightningStrike.GetComponent<Transform>().Position = boss.GetComponent<Transform>().Position;
                lightningStrike.GetComponent<AnimationComponent>().PlayAnimation();
                SoundManager.PlaySound("lightning");
            }
            List<GameObject> targetEnemies = ObjectManager.FindAllObjectsByTag("enemy");
            foreach (var enemy in targetEnemies)
            {
                enemy.GetComponent<HealthComponent>().TakeDamage(2);
                GameObject lightningStrike = GameObjectFactory.CreateLightningStrike();
                ObjectManager.AddGameObject(lightningStrike);
                lightningStrike.GetComponent<Transform>().Position = enemy.GetComponent<Transform>().Position;
                lightningStrike.GetComponent<AnimationComponent>().PlayAnimation();
                SoundManager.PlaySound("lightning");
            }
            playerController.EndTurn();
        }
    }

    /// <summary>
    /// This class is responsible for managing the force scroll.
    /// </summary>
    public class ForceScroll : ItemComponent
    {
        public ForceScroll(string name, string description) : base(name, description)
        {
        }

        public override void UseItem()
        {
            // Apply opposing for to each enemy.
            var tilemap = ObjectManager.Find("tilemap").GetComponent<Tilemap>();
            var player = ObjectManager.Find("player");
            var playerTransform = player.GetComponent<Transform>();
            var enemies = ObjectManager.FindAllObjectsByTag("enemy");
            var boss = ObjectManager.Find("Boss");
            if (boss != null)
            {
                // Spawn hurricanes around boss.
                var bossTransform = boss.GetComponent<Transform>();
                GameObject hurricane = GameObjectFactory.CreateHurricane();
                ObjectManager.AddGameObject(hurricane);
                hurricane.GetComponent<Transform>().Position = bossTransform.Position;
                hurricane.GetComponent<AnimationComponent>().PlayAnimation();
                SoundManager.PlaySound("swoosh");
                Vector2 direction = bossTransform.Position - playerTransform.Position;
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }
                bossTransform.Position += direction * 96;
                bossTransform.Position = new Vector2(
                    Math.Clamp(bossTransform.Position.X, 0, tilemap.MapWidth * tilemap.TileWidth),
                    Math.Clamp(bossTransform.Position.Y, 0, tilemap.MapHeight * tilemap.TileHeight)
                );
            }
            foreach (var enemy in enemies)
            {
                // Spawn hurricanes around each enemy.
                var enemyTransform = enemy.GetComponent<Transform>();
                GameObject hurricane = GameObjectFactory.CreateHurricane();
                ObjectManager.AddGameObject(hurricane);
                hurricane.GetComponent<Transform>().Position = enemy.GetComponent<Transform>().Position;
                hurricane.GetComponent<AnimationComponent>().PlayAnimation();
                SoundManager.PlaySound("swoosh");
                Vector2 direction = enemyTransform.Position - playerTransform.Position;
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }
                enemyTransform.Position += direction * 96;
                enemyTransform.Position = new Vector2(
            Math.Clamp(enemyTransform.Position.X, 0, tilemap.MapWidth * tilemap.TileWidth),
            Math.Clamp(enemyTransform.Position.Y, 0, tilemap.MapHeight * tilemap.TileHeight)
        );
                if (enemy.GetComponent<BaseEnemyAI>() != null)
                {
                    enemy.GetComponent<BaseEnemyAI>().ClampPosition();
                }

            }
            Remove();
        }
    }


    /// <summary>
    /// This class is responsible for the the fireball projectile.
    /// </summary>
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

            // Check for collisions with enemies.
            var boss = ObjectManager.Find("Boss");
            if (boss != null && TilePosition(boss.GetComponent<Transform>().Position) == tilePosition)
            {
                var bossHealth = boss.GetComponent<HealthComponent>();
                bossHealth?.TakeDamage(3);
                GameObject?.Destroy();
                return;
            }
            List<GameObject> enemies = ObjectManager.FindAllObjectsByTag("enemy");
            foreach (var enemy in enemies)
            {
                if (TilePosition(enemy.GetComponent<Transform>().Position) == tilePosition)
                {
                    var enemyHealth = enemy.GetComponent<HealthComponent>();
                    enemyHealth?.TakeDamage(3);
                    GameObject?.Destroy();
                    
                    return;
                }
            }

            // Check for collisions with obstacles.
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

    /// <summary>
    /// This class is responsible for managing the player's weapon, firing etc.
    /// </summary>
    public class Weapon : Component
    {
        string name;
        /// <summary>
        /// Chosen direction for the weapon, set by the player.
        /// </summary>
        public Vector2 chosenDirection = Vector2.Zero;
        // Selected direction for the weapon, is adjusted based on input before firing.
        private Vector2 selectedDirection = new Vector2(0, -1); 
        GameObject fireball;
        bool hasFired = false;

        public bool waitingForDirection = false;
        // Tuple to contain arrow object and direction.
        List<(GameObject arrow, Vector2 direction)> directionArrows = new();
        // Set the default colors for the direction arrows.
        Color selectedColor = Color.Gray;
        Color defaultColor = Color.White;

        public Weapon(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// This method is responsible for getting the direction input from the player.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetDirectionInput()
        {
            KeyboardState currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.W)) return new Vector2(0, -1);
            if (currentState.IsKeyDown(Keys.A)) return new Vector2(-1, 0);
            if (currentState.IsKeyDown(Keys.S)) return new Vector2(0, 1);
            if (currentState.IsKeyDown(Keys.D)) return new Vector2(1, 0);

            // If there's no input, return the last selected direction.
            return selectedDirection; 
        }

        public override void Update()
        {
            if (GameObject == null) return;
            var playerController = GameObject.GetComponent<PlayerController>();

            if (waitingForDirection)
            {
                // Disables player movement while waiting for direction.
                playerController.IsShooting = true;

                Vector2 input = GetDirectionInput();
                if (input != selectedDirection)
                {
                    // If the input changes, update the arrows.
                    selectedDirection = input;
                    UpdateDirectionArrows();
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Space) && selectedDirection != Vector2.Zero)
                {
                    // If the player has selected a direction and pressed space, shoot the weapon.
                    chosenDirection = selectedDirection;
                    playerController.IsShooting = false;
                    ShootWeapon();
                    waitingForDirection = false;
                    RemoveDirectionArrows();
                    playerController.EndTurn();
                }
            }
        }

        /// <summary>
        /// This method is responsible for creating a fireball and shooting it in the chosen direction.
        /// </summary>
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
            SoundManager.PlaySound("fireshot");
            ObjectManager.Find("player").GetComponent<PlayerController>().EndTurn();
        }

        /// <summary>
        /// Create's directional arrows around the player.
        /// </summary>
        public void CreateDirectionArrows()
        {
            // Directions player's can choose.
            Vector2[] choices =
            {
            new Vector2(1, 0),  
            new Vector2(-1, 0), 
            new Vector2(0, -1), 
            new Vector2(0, 1)   
        };
            
            // Rotations for the arrows, based on direction.
            float[] rotations =
        {
            -MathHelper.PiOver2, 
            MathHelper.PiOver2,  
            MathHelper.Pi,       
            0f                  
        };

            Vector2 playerPosition = ObjectManager.Find("player").GetComponent<Transform>().Position;

            for (int i = 0; i < choices.Length; i++)
            {
                GameObject arrow = GameObjectFactory.CreateTurnArrow();
                Transform arrowTransform = arrow.GetComponent<Transform>();
                Sprite sprite = arrow.GetComponent<Sprite>();

                // Offset, considering the arrow's rotation starts downward.
                Vector2 offset = Vector2.Zero;
                if (choices[i].X == 1) offset = new Vector2(0, 32);  
                if (choices[i].X == -1) offset = new Vector2(32, 0); 
                if (choices[i].Y == -1) offset = new Vector2(32, 32);  
                if (choices[i].Y == 1) offset = new Vector2(0, 0);   

                // Mulitply position by tile size and add offset.
                arrowTransform.Position = playerPosition + (choices[i] * 32f) + offset;
                arrowTransform.Rotation = rotations[i];

                sprite.Color = defaultColor;

                directionArrows.Add((arrow, choices[i]));
                ObjectManager.AddGameObject(arrow);
            }
        }

        /// <summary>
        /// This method is responsible for updating the directional arrows around the player.
        /// </summary>
        public void UpdateDirectionArrows()
        {
            Vector2 playerPosition = ObjectManager.Find("player").GetComponent<Transform>().Position;

            // Iterate through the tuple list and update the arrows.
            foreach (var (arrow, direction) in directionArrows)
            {
                // Apply the offset when updating arrows.
                Vector2 offset = Vector2.Zero;
                if (direction.X == 1) offset = new Vector2(0, 32);  
                if (direction.X == -1) offset = new Vector2(32, 0); 
                if (direction.Y == -1) offset = new Vector2(32, 32); 
                if (direction.Y == 1) offset = new Vector2(0, 0);  

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

        /// <summary>
        /// This method is responsible for removing the arrows after the player has fired the weapon.
        /// </summary>
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
