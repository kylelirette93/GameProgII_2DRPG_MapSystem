using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


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
            if (currentState.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.W)) movement.Y -= 16;
            if (currentState.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.S)) movement.Y += 16;
            if (currentState.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.A)) movement.X -= 16;
            if (currentState.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.D)) movement.X += 16;

            if (movement != Vector2.Zero && player != null)
            {
                // Normalize the movement vector.
                Vector2 newPosition = player.GetComponent<Transform>().Position + movement;
                if (CanMoveTo(newPosition))
                {
                    player.GetComponent<Transform>()?.Translate(movement);
                    isTurn = false;
                    TurnManager.EndTurn();
                }
            }

            // Assign the current state to the previous state, this is used to check for key presses.
            previousState = currentState;
        }
        private bool CanMoveTo(Vector2 newPosition)
        {
            tilemap = ObjectManager.Find("tilemap")?.GetComponent<Tilemap>();
     
            if (tilemap != null)
            {
                // Get the tile position based on the new position.
                int tileX = (int)(newPosition.X / tilemap.TileWidth);
                int tileY = (int)(newPosition.Y / tilemap.TileHeight);

                // Check if the tile is within bounds and walkable.
                if (tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
                {
                    Tile tile = tilemap.Tiles[tileX, tileY];
                    if (tile.IsExit)
                    {
                        OnExitTile?.Invoke();
                        return false;
                    }
                    return tile.IsWalkable;
                }
            }
            return false;
        }

        public override void TakeTurn()
        {
            isTurn = true;
        }
    }
}




