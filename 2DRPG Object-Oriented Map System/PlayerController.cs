﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Player Controller is a component that handles player input and movement.
    /// </summary>

    public class PlayerController : Component, ITurnTaker
    {
        /// <summary>
        /// Event for when the player reaches the exit tile.
        /// </summary>
        public event Action OnExitTile;
        private float movementSpeed = 5f;
        private GameObject player;
        private Transform enemyTransform;
        private BaseEnemyAI enemyAI;
        private KeyboardState previousState;
        private KeyboardState currentState;
        private Tilemap tilemap;
        public bool IsTurn { get => isTurn; }
        bool isTurn = false;
        public int X { get => (int)player.GetComponent<Transform>().Position.X; }
        public int Y { get => (int)player.GetComponent<Transform>().Position.Y; }

        public string Id => "Player";

        /// <summary>
                /// Initializes the previous state of the keyboard.
                /// </summary>

        public PlayerController(string name)
        {
            Name = name;
            previousState = Keyboard.GetState();
        }

        /// <summary>
        /// Update method is responsible for checking player input and moving the player.
        /// </summary>

        public override void Update()
        {
            if (player == null)
            {
                player = ObjectManager.Find("player");
            }
            if (enemyTransform == null)
            {
                enemyTransform = ObjectManager.Find("enemy")?.GetComponent<Transform>();
            }
            if (enemyAI == null)
            {
                   enemyAI = ObjectManager.Find("enemy")?.GetComponent<MeleeEnemyAI>();
            }
            if (isTurn)
            {
                HandleInput();
            }
        }
        private void HandleInput()
        {
            // Create a movement vector.
            Vector2 movement = Vector2.Zero;
            // Update the current state of the keyboard.
            currentState = Keyboard.GetState();
            // Compare the current state with the previous state to check for key presses.
            if (currentState.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.W)) movement.Y -= 32;
            if (currentState.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.S)) movement.Y += 32;
            if (currentState.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.A)) movement.X -= 32;
            if (currentState.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.D)) movement.X += 32;

            if (movement != Vector2.Zero && player != null)
            {
                // Normalize the movement vector.
                Vector2 newPosition = player.GetComponent<Transform>().Position + movement;
                if (CanMoveTo(newPosition))
                {
                    player.GetComponent<Transform>()?.Translate(movement);
                    int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                    int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);
                    EndTurn();
                }
            }
            // Assign the current state to the previous state, this is used to check for key presses.
            previousState = currentState;
        }
        private bool CanMoveTo(Vector2 newPosition)
        {
            player = ObjectManager.Find("player");
            var exit = ObjectManager.Find("exit");
            tilemap = ObjectManager.Find("tilemap")?.GetComponent<Tilemap>();

            if (player != null)
            {
                var playerCollider = player.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    var tempPlayerBounds = new Rectangle(
                      (int)newPosition.X,
                      (int)newPosition.Y,
                      playerCollider.Bounds.Width,
                      playerCollider.Bounds.Height
                    );

                    // Create a copy of the enemies list to avoid concurrent modification
                    var enemies = ObjectManager.FindAllObjectsByTag("enemy").ToList(); 
                    foreach (var enemyObject in enemies)
                    {
                        if (enemyObject != null)
                        {
                            var enemyCollider = enemyObject.GetComponent<Collider>();
                            if (enemyCollider != null && tempPlayerBounds.Intersects(enemyCollider.Bounds))
                            {
                                var enemyHealth = enemyObject.GetComponent<HealthComponent>();
                                enemyAI = enemyObject.GetComponent<MeleeEnemyAI>();
                                enemyHealth?.TakeDamage(1);
                                enemyAI?.Stun();
                                EndTurn();
                                return false;
                            }
                        }
                    }

                    var items = ObjectManager.FindAllObjectsByTag("item").ToList();
                    foreach (var itemObject in items)
                    {

                       if (itemObject != null)
                        {
                            var itemCollider = itemObject.GetComponent<Collider>();
                            if (itemCollider != null && tempPlayerBounds.Intersects(itemCollider.Bounds))
                            {
                                Debug.WriteLine("Object picked up:" + itemObject);
                                ObjectManager.Find("player")?.GetComponent<Inventory>().AddItem(itemObject.GetComponent<ItemComponent>());
                                itemObject.Destroy();
                                return true;
                            }
                        }
                    }
                    if (exit != null)
                    {
                        var exitCollider = exit.GetComponent<Collider>();
                        if (exitCollider != null && tempPlayerBounds.Intersects(exitCollider.Bounds))
                        {
                            // Check if all enemies are dead.
                            if (!enemies.Any(enemy => enemy.GetComponent<HealthComponent>()?.CurrentHealth > 0 &&
                            enemy.GetComponent<GhostEnemyAI>() == null))
                            {
                                OnExitTile?.Invoke();
                                return true; // Allow movement if all enemies are dead
                            }
                            else
                            {
                                return false; // Do not allow movement if enemies are alive
                            }
                        }
                    }
                }
            }

            // Tilemap collision check

            if (tilemap != null)
            {
                return CheckTilemapCollision(tilemap, newPosition);
            }
            return true;
        }

        /// <summary>
        /// This method checks if the player can move to a new position on the tilemap.
        /// </summary>
        /// <param name="tilemap"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        private bool CheckTilemapCollision(Tilemap tilemap, Vector2 newPosition)
        {
            // Get the tile coordinates based on the new position.
            int tileX = (int)(newPosition.X / tilemap.TileWidth);
            int tileY = (int)(newPosition.Y / tilemap.TileHeight);

            if (tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
            {
                Tile tile = tilemap.Tiles[tileX, tileY];
                return tile.IsWalkable;
            }
            return false;
        }
        public void EndTurn()
        {
            isTurn = false;
        }
    
        public void TakeTurn()
        {
            if (this == null)             
            {
                return;
            }
            // Debug.WriteLine("Player turn.");
        }

        public void StartTurn()
        {          
            isTurn = true;
            Debug.WriteLine("Player started turn!");
        }
    }
}