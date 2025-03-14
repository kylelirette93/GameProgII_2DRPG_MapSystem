using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Player Controller is a component that handles player input and movement.
    /// </summary>

    public class PlayerController : TurnComponent
    {
        /// <summary>
        /// Event for when the player reaches the exit tile.
        /// </summary>

        public event Action OnExitTile;
        private float movementSpeed = 5f;
        private GameObject player;
        private Transform enemyTransform;
        private EnemyAI enemyAI;
        private KeyboardState previousState;
        private KeyboardState currentState;
        private Tilemap tilemap;
        bool isTurn = false;

        /// <summary>
        /// Initializes the previous state of the keyboard.
        /// </summary>

        public PlayerController()
        {
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
                    EndTurn();
                }
            }
            // Assign the current state to the previous state, this is used to check for key presses.
            previousState = currentState;
        }
        private bool CanMoveTo(Vector2 newPosition)
        {
            var player = ObjectManager.Find("player");
            var exit = ObjectManager.Find("exit");
            var tilemap = ObjectManager.Find("tilemap")?.GetComponent<Tilemap>();

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

                    var enemies = ObjectManager.FindAllObjectsByTag("enemy").ToList(); // Create a copy
                    foreach (var enemyObject in enemies)
                    {
                        if (enemyObject != null)
                        {
                            var enemyCollider = enemyObject.GetComponent<Collider>();
                            if (enemyCollider != null && tempPlayerBounds.Intersects(enemyCollider.Bounds))
                            {
                                var enemyHealth = enemyObject.GetComponent<HealthComponent>();
                                enemyAI = enemyObject.GetComponent<EnemyAI>();
                                enemyHealth?.TakeDamage(10);
                                enemyAI?.Stun();                              
                                return false;
                            }
                        }
                    }
                    if (exit != null)
                    {
                        var exitCollider = exit.GetComponent<Collider>();
                        if (exitCollider != null && tempPlayerBounds.Intersects(exitCollider.Bounds))
                        {
                            OnExitTile?.Invoke();
                        }
                    }
                }
            }

            // Tilemap collision check

            if (tilemap != null)
            {
                int tileX = (int)(newPosition.X / tilemap.TileWidth);
                int tileY = (int)(newPosition.Y / tilemap.TileHeight);

                if (tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
                {
                    Tile tile = tilemap.Tiles[tileX, tileY];
                    return tile.IsWalkable;
                }
            }
            return false;
        }

        private void EndTurn()
        {
            isTurn = false;
            TurnManager.EndTurn();
        }
        public override void TakeTurn()
        {
            isTurn = true;
            Debug.WriteLine("Player turn.");
        }
    }
}