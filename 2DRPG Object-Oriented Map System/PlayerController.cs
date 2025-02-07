using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace _2DRPG_Object_Oriented_Map_System
{
    public class PlayerController : Component
    {
        private float movementSpeed = 5f;
        private GameObject player;
        private KeyboardState previousState;

        public override void Update()
        {
            if (player == null)
            {
                player = GameManager.Find("player");
            }

            if (player != null)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            Vector2 movement = Vector2.Zero;
            KeyboardState currentState = Keyboard.GetState();

            // Check for key presses
            if (currentState.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.W)) movement.Y -= 16;
            if (currentState.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.S)) movement.Y += 16;
            if (currentState.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.A)) movement.X -= 16;
            if (currentState.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.D)) movement.X += 16;

            if (movement != Vector2.Zero && player != null)
            {
                Vector2 newPosition = player.GetComponent<Transform>().Position + movement;
                if (CanMoveTo(newPosition))
                {
                    player.GetComponent<Transform>()?.Translate(movement);
                }
            }

            // Update the previous state
            previousState = currentState;
        }

        private bool CanMoveTo(Vector2 newPosition)
        {
            Tilemap tilemap = GameManager.Find("tilemap")?.GetComponent<Tilemap>();

            if (tilemap != null)
            {
                // Get the tile position based on newPosition
                int tileX = (int)(newPosition.X / tilemap.TileWidth);
                int tileY = (int)(newPosition.Y / tilemap.TileHeight);

                // Check if the tile is within bounds and walkable
                if (tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
                {
                    Tile tile = tilemap.Tiles[tileX, tileY];
                    return tile.IsWalkable;
                }
            }
            return false;
        }
    }
}




