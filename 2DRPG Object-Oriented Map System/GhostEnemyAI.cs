using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Security.AccessControl;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// This class inherits from the base. It handle's ghost enemys that can move through obstacles.
    /// </summary>
    public class GhostEnemyAI : BaseEnemyAI
    {
        
        public GhostEnemyAI(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
        }

        public override void Initialize()
        {
            base.Initialize();            
        }

        public override void UpdateTarget()
        {
            if (playerTransform == null || enemyTransform == null)
            {
                Debug.WriteLine("Can't find target, somethings null!");
                return;
            }
            enemyPosition = new Point((int)(enemyTransform.Position.X / tilemap.TileWidth),
                (int)(enemyTransform.Position.Y / tilemap.TileHeight));
            playerPosition = new Point((int)(playerTransform.Position.X / tilemap.TileWidth),
                (int)(playerTransform.Position.Y / tilemap.TileHeight));
        }

        public override void FollowPlayer()
        {
            UpdateTarget();

            Vector2 direction = playerTransform.Position - enemyTransform.Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 newPosition = enemyTransform.Position + direction * 32;

            // Round to nearest tile boundary
            newPosition = new Vector2(
                (int)(Math.Round(newPosition.X / tilemap.TileWidth) * tilemap.TileWidth),
                (int)(Math.Round(newPosition.Y / tilemap.TileHeight) * tilemap.TileHeight)
            );

            Point newPoint = new Point((int)(newPosition.X / tilemap.TileWidth), (int)(newPosition.Y / tilemap.TileHeight));
            Point playerPoint = new Point((int)(playerTransform.Position.X / tilemap.TileWidth), (int)(playerTransform.Position.Y / tilemap.TileHeight));

            if (newPoint != playerPoint)
            {
                enemyTransform.Position = newPosition;
            }

            if (IsAdjacentToPlayer())
            {
                ChangeState(EnemyState.Attack);
                DealDamage();
                EndTurn();
                return;
            }
            EndTurn();
        }


        protected override Vector2 FindValidPosition(Vector2 originalPosition)
        {
            // Ghosts ignore collisions, so they always move towards the player.
            return originalPosition;
        }

        protected override bool IsWalkableTile(int tileX, int tileY)
        {
            // Ghosts can move through walls, so all tiles are walkable.
            return true;
        }

        public override Vector2 GetSpawnPosition(Vector2 playerPosition, float minDistance)
        {
            Vector2 spawnPosition = new Vector2();
            bool validPosition = false;

            while (!validPosition)
            {
                // Generate a random tile position. Based on the tile position, calculate the spawn position.
                int tileX = _random.Next(1, 768 / 32);
                int tileY = _random.Next(1, 448 / 32);
                spawnPosition = new Vector2(tileX * 32, tileY * 32);

                // Check if the spawn position is far enough from the player. No need to check for walkability.
                if (Vector2.Distance(spawnPosition, playerPosition) >= minDistance)
                {
                    validPosition = true;
                }
            }

            return spawnPosition;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void DealDamage()
        {
            base.DealDamage();
        }

        public override void Stun()
        {
            base.Stun();
        }

    }
}