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
        private Tilemap tilemap;
        private Random _random = new Random();
        private bool _initialized = false;
        private bool isTurn = false;
        private string _name;
        private int stunnedCounter;
        /// <summary>
        /// Sets the default state of the enemy AI.
        /// </summary>
        public EnemyState CurrentState { get; set; } = EnemyState.Follow;

        /// <summary>
        /// Constructor for the EnemyAI class. Has two names, one for the GameObject and one for the EnemyAI to be managed in turn component. 
        /// Not my finest solution, but works for now.
        /// </summary>
        /// <param name="name"></param>
        public EnemyAI(string name)
        {
            Name = "Enemy";
            _name = name;
            stunnedCounter = 0;
        }

        /// <summary>
        /// Simple state machine for the enemy AI.
        /// </summary>
        public enum EnemyState
        {
            Stunned,
            Follow,
            Attack
        }

        /// <summary>
        /// This method changes the state of the enemy AI.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(EnemyState state)
        {
            CurrentState = state;
        }

        /// <summary>
        /// Initialize the enemy AI with the player, enemy, and tilemap references
        /// </summary>
        public void Initialize()
        {
            player = ObjectManager.Find("player");
            enemy = ObjectManager.Find(_name);
            tilemap = ObjectManager.Find("tilemap")?.GetComponent<Tilemap>();
            if (player != null)
            {
                playerTransform = player.GetComponent<Transform>();
                player.GetComponent<PlayerController>().OnExitTile += CalculateEnemySpawn;
            }
            if (enemy != null)
            {
                enemyTransform = enemy.GetComponent<Transform>();
                CalculateEnemySpawn();
            }
        }

        /// <summary>
        /// This method is called when the enemy AI is supposed to follow the player. It checks if the enemy can move to the next tile and moves it.
        /// </summary>
        private void FollowPlayer()
        {
            Vector2 direction = playerTransform.Position - enemyTransform.Position;
            Vector2 tileDirection = new Vector2(Math.Sign(direction.X), Math.Sign(direction.Y));
            Vector2 nextPosition = enemyTransform.Position + tileDirection * 32f;

            if (CanMoveTo(nextPosition))
            {
                enemyTransform.Position = nextPosition;
                EndTurn();
            }
            else
            {
                FindValidSpace();
            }
        }

        /// <summary>
        /// This method finds a valid space for the enemy to move to.
        /// </summary>
        /// <returns></returns>
        Vector2 FindValidSpace()
        {
            // Initialize array of directions.          
            Vector2[] directions = new Vector2[] {
                new Vector2(0, -32),
                new Vector2(0, 32),
                new Vector2(-32, 0),
                new Vector2(32, 0),
                new Vector2(-32, -32),
                new Vector2(32, 32),
                new Vector2(-32, 32),
                new Vector2(32, -32) };
            int maxAttempts = 9; 
            int attempts = 0; 
            // Iterate through directions.
            // If a valid space is found, move the enemy to that space, check for player adjacency, and end the turn.
            while (attempts < maxAttempts) 
            { 
                foreach (var direction in directions) 
                { 
                    Vector2 nextPosition = enemyTransform.Position + direction;
                    if (CanMoveTo(nextPosition)) 
                    { 
                        enemyTransform.Position = nextPosition;
                        if (IsAdjacentToPlayer())
                        { 
                            ChangeState(EnemyState.Attack); 
                        } 
                        EndTurn();
                        return direction; 
                    } 
                } attempts++;
            } 
            return Vector2.Zero;
        
        
        }
        /// <summary>
        /// This method checks if the enemy is adjacent to the player.
        /// </summary>
        /// <returns></returns>
            bool IsAdjacentToPlayer()
            {
                if (enemy != null && player != null && tilemap != null)
                {
                // Calculate tile positions.
                    int enemyTileX = (int)(enemyTransform.Position.X / tilemap.TileWidth);
                    int enemyTileY = (int)(enemyTransform.Position.Y / tilemap.TileHeight);
                    int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                    int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                    // Check for adjacency to player.
                    if ((enemyTileX == playerTileX && Math.Abs(enemyTileY - playerTileY) == 1) ||
                        (enemyTileY == playerTileY && Math.Abs(enemyTileX - playerTileX) == 1))
                    {
                        return true; 
                    }
                }
                return false; 
            }

            bool CanMoveTo(Vector2 newPosition)
            {
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

                        // Check if player tries to move into the same tile as the enemy.
                        if (enemyTileX == playerTileX && enemyTileY == playerTileY)
                        {
                            return false; // Collision with player.
                        }

                        // Check if the enemy can move to the new position.
                        if (enemyTileX >= 0 && enemyTileX < tilemap.Tiles.GetLength(0) && enemyTileY >= 0 && enemyTileY < tilemap.Tiles.GetLength(1))
                        {
                            Tile tile = tilemap.Tiles[enemyTileX, enemyTileY];
                            if (!tile.IsWalkable)
                            {
                                return false; 
                            }
                        }
                        else
                        {
                            // Return false if the enemy is out of bounds.
                            return false; 
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
                                    // If enemy's new position is the same as another enemy's position, return false.
                                        return false; 
                                    }
                                }
                            }
                        }
                        // If no collision is detected, return true.
                        return true; 
                    }
                }
                // If enemy or player is null, return false.
                return false; 
            }
        /// <summary>
        /// This method is called when the player exits a tile, to recalculate the enemy spawn position.
        /// </summary>
            private void CalculateEnemySpawn()
            {
                enemyTransform.Position = GetSpawnPosition(playerTransform.Position, 256);
            }

        /// <summary>
        /// This method calculates a spawn position for the enemy, based on the player's position and a minimum distance.
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
            public Vector2 GetSpawnPosition(Vector2 playerPosition, float minDistance)
            {
                Vector2 spawnPosition = new Vector2();
                bool validPosition = false;

                while (!validPosition)
                {
                // Generate a random tile position. Based on the tile position, calculate the spawn position.
                    int tileX = _random.Next(1, 768 / 32); 
                    int tileY = _random.Next(1, 448 / 32); 
                    spawnPosition = new Vector2(tileX * 32, tileY * 32);

                    // Check if the spawn position is far enough from the player and is walkable.
                    if (Vector2.Distance(spawnPosition, playerPosition) >= minDistance && IsWalkableTile(tileX, tileY))
                    {
                        validPosition = true;
                    }
                }

                return spawnPosition;
            }

        /// <summary>
        /// This method checks if a tile is walkable.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
            private bool IsWalkableTile(int tileX, int tileY)
            {
            // Check if the tile is within bounds and is walkable.
                if (tilemap != null && tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
                {
                    Tile tile = tilemap.Tiles[tileX, tileY];
                    return tile.IsWalkable;
                }
                return false;
            }

           
        public override void Update()
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }
            if (!isTurn)
            {
                return;
            }
            switch (CurrentState)
            {
                case EnemyState.Follow:
                    Debug.WriteLine("Enemy Turn");
                    FollowPlayer();
                    break;

                case EnemyState.Attack:
                    var playerHealth = player.GetComponent<HealthComponent>();
                    playerHealth?.TakeDamage(1);
                    ChangeState(EnemyState.Follow);
                    EndTurn();
                    break;

                case EnemyState.Stunned:
                    EndTurn();
                    break;
            }
        }

        private void EndTurn()
        {
            isTurn = false;
            TurnManager.EndTurn();
        }

        public void Stun()
        {
            var enemyAnimation = ObjectManager.Find(GameObject.Tag).GetComponent<AnimationComponent>();
            enemyAnimation.isLooping = true;
            enemyAnimation.PlayAnimation();
            stunnedCounter = 3; // Reset the stunned counter
            ChangeState(EnemyState.Stunned);
        }
        public override void TakeTurn()
        {
            if (isTurn)
            {
                // Prevent's multiple turns from being taken.
                return;
            }

            isTurn = true;

            // Check's the state's and takes the appropriate action.
            if (CurrentState == EnemyState.Follow && IsAdjacentToPlayer())
            {
                ChangeState(EnemyState.Attack);
                return; 
            }

            if (CurrentState == EnemyState.Stunned && stunnedCounter > 0)
            {
                stunnedCounter--; 
                if (stunnedCounter == 0)
                {
                    var enemyAnimation = ObjectManager.Find(GameObject.Tag).GetComponent<AnimationComponent>();
                    enemyAnimation.isLooping = false;
                    enemyAnimation.PlayAnimation();
                    CurrentState = EnemyState.Follow;
                }
                EndTurn();
            }
            else
            {
                if (CurrentState == EnemyState.Follow)
                {
                    FollowPlayer();
                }
                else if (CurrentState == EnemyState.Attack)
                {
                    var playerHealth = player.GetComponent<HealthComponent>();
                    playerHealth?.TakeDamage(1);
                    ChangeState(EnemyState.Follow);
                    EndTurn();
                }
            }
        }
    }
}


        