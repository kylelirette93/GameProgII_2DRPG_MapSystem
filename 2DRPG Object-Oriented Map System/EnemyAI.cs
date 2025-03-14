using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class EnemyAI : TurnComponent
    {
        private GameObject enemy;
        private GameObject player;
        private Transform playerTransform;
        private Transform enemyTransform;
        private Random _random = new Random();
        bool _initialized = false;
        bool isTurn = false;
        string _name;
        int stunnedCounter;
        public EnemyState CurrentState { get; set; } = EnemyState.Follow;   

        public EnemyAI(string name)
        {
            this._name = name;
            stunnedCounter = 0;
        }

        public enum EnemyState
        {
            Stunned,
            Follow,
            Attack
        }

        public void ChangeState(EnemyState state)
        {
            CurrentState = state;
        }

        public void Initialize()
        {
            if (player == null)
            {
                player = ObjectManager.Find("player");
                if (player != null)
                {
                    Debug.WriteLine("EnemyAI: Player found.");
                    playerTransform = player.GetComponent<Transform>();
                    player.GetComponent<PlayerController>().OnExitTile += CalculateEnemySpawn;
                }
            }

            if (enemy == null)
            {
                enemy = ObjectManager.Find(_name);
                if (enemy != null)
                {
                    enemyTransform = enemy.GetComponent<Transform>();
                    CalculateEnemySpawn();
                }
            }
        }

        private void FollowPlayer()
        {
            Vector2 direction = playerTransform.Position - enemyTransform.Position;
            // Calculate tile-based direction.
            Vector2 tileDirection = new Vector2(
        Math.Sign(direction.X),
        Math.Sign(direction.Y)
      );

            Vector2 nextPosition = enemyTransform.Position + tileDirection * 32f;

            if (CanMoveTo(nextPosition))
            {
                enemyTransform.Position = nextPosition;
                EndTurn();
                isTurn = false;
            }
            else
            {
                FindValidSpace();
            }
        }
        Vector2 FindValidSpace()
        {
            // Check all 8 directions and find an empty space.
            Vector2[] directions = new Vector2[]
                {
                    new Vector2(0, -32),
                    new Vector2(0, 32),
                    new Vector2(-32, 0),
                    new Vector2(32, 0),
                    new Vector2(-32, -32),
                    new Vector2(32, 32),
                    new Vector2(-32, 32),
                    new Vector2(32, -32)
                };

            foreach (var direction in directions)
            {
                Vector2 nextPosition = enemyTransform.Position + direction;
                if (CanMoveTo(nextPosition))
                {
                   enemyTransform.Position = nextPosition;
                    EndTurn();
                    return direction;
                }
            }
            return Vector2.Zero;
        }

        bool CanMoveTo(Vector2 newPosition)
        {
            var player = ObjectManager.Find("player");
            var enemy = ObjectManager.Find(_name);
            var tilemap = ObjectManager.Find("tilemap")?.GetComponent<Tilemap>();

            if (enemy != null && player != null && tilemap != null)
            {
                var enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    // Calculate tile positions.
                    int enemyTileX = (int)(newPosition.X / tilemap.TileWidth);
                    int enemyTileY = (int)(newPosition.Y / tilemap.TileHeight);
                    int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                    int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                    // Player collision check: check if tile positions are the same.
                    if (enemyTileX == playerTileX && enemyTileY == playerTileY)
                    {
                        ChangeState(EnemyState.Attack);
                        EndTurn();
                        return false; // Collision with player.
                    }

                    // Tilemap collision check (as before).
                    if (enemyTileX >= 0 && enemyTileX < tilemap.Tiles.GetLength(0) && enemyTileY >= 0 && enemyTileY < tilemap.Tiles.GetLength(1))
                    {
                        Tile tile = tilemap.Tiles[enemyTileX, enemyTileY];
                        if (!tile.IsWalkable)
                        {
                            return false; // Collision with tile.
                        }
                    }
                    else
                    {
                        return false; // Out of bounds.
                    }

                    // Check collision with other enemies.
                    foreach (var otherEnemy in ObjectManager.FindAll())
                    {
                        if (otherEnemy != enemy && otherEnemy.GetComponent<EnemyAI>() != null)
                        {
                            var otherEnemyCollider = otherEnemy.GetComponent<Collider>();
                            if (otherEnemyCollider != null)
                            {
                                int otherEnemyTileX = (int)(otherEnemy.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                                int otherEnemyTileY = (int)(otherEnemy.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                                if (enemyTileX == otherEnemyTileX && enemyTileY == otherEnemyTileY)
                                {
                                    return false; // Collision with other enemy.
                                }
                            }
                        }
                    }
                    return true; // No collision.
                }
            }
            return false; // Enemy or collider not found.
        }

        private void CalculateEnemySpawn()
        {
            enemyTransform.Position = GetSpawnPosition(playerTransform.Position, 256);
        }

        public Vector2 GetSpawnPosition(Vector2 playerPosition, float minDistance)
        {
            Random random = new Random();
            Vector2 spawnPosition;

            do
            {
                int tileX = random.Next(1, 768 / 32); // Ensure tileX is a tile index.
                int tileY = random.Next(1, 448 / 32); // Ensure tileY is a tile index.
                spawnPosition = new Vector2(tileX * 32, tileY * 32);
            }
            while (Vector2.Distance(spawnPosition, playerPosition) < minDistance);
            return spawnPosition;
        }
        public override void Update()
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }
            if (CurrentState == EnemyState.Follow && isTurn)
            {
                Debug.WriteLine("Enemy Turn");
                FollowPlayer();
            }
            else if (CurrentState == EnemyState.Attack && isTurn)
            {
                var playerHealth = player.GetComponent<HealthComponent>();
                playerHealth?.TakeDamage(10);
                ChangeState(EnemyState.Follow);
                EndTurn();
            }
            else if (CurrentState == EnemyState.Stunned && isTurn)
            {
                EndTurn();
            }
        }

        private void EndTurn()
        {
            isTurn = false;
            TurnManager.EndTurn();
        }

        public void Stun()
        {
            ChangeState(EnemyState.Stunned);
            stunnedCounter = 2;
            EndTurn();
        }
        public override void TakeTurn()
        {
            isTurn = true;
            if (CurrentState == EnemyState.Stunned && stunnedCounter > 0)
            {
                stunnedCounter--; // Decrement counter.
                if (stunnedCounter == 0)
                {
                    CurrentState = EnemyState.Follow;
                }
                EndTurn();
            }
        }
    }
}

