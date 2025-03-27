using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace _2DRPG_Object_Oriented_Map_System
{
    public abstract class BaseEnemyAI : Component, ITurnTaker
    {
        protected GameObject enemy; 
        protected GameObject player; 
        protected Transform playerTransform;
        protected Transform enemyTransform;
        protected Tilemap tilemap;
        protected Random _random = new Random();
        protected bool _initialized = false;
        protected bool isTurn = false;
        protected string _name;
        protected int stunnedCounter;

        protected PathNode[,] nodeMap;
        protected List<Point> currentPath;
        protected int currentPathIndex;
        protected Point targetPoint;
        protected Pathfinder pathfinder;
        protected Point enemyPosition;
        protected Point playerPosition;
        protected HashSet<Point> occupiedTiles;

        /// <summary>
        /// Sets the default state of the enemy AI.
        /// </summary>
        public EnemyState CurrentState { get; set; } = EnemyState.Follow;

        public bool IsTurn => isTurn;

        public string Id { get; set; }

        /// <summary>
        /// Constructor for the EnemyAI class. Has two names, one for the GameObject and one for the EnemyAI to be managed in turn component. 
        /// Not my finest solution, but works for now.
        /// </summary>
        /// <param name="name"></param>
        public BaseEnemyAI(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            _name = name;
            stunnedCounter = 0;
            pathfinder = new Pathfinder();
        }

        public virtual void UpdateTarget()
        {
            if (tilemap == null || playerTransform == null || enemyTransform == null && pathfinder == null)
            {
                Debug.WriteLine("Can't find target, somethings null!");
                return; // Handle null cases
            }
            enemyPosition = new Point((int)(enemyTransform.Position.X / tilemap.TileWidth),
                (int)(enemyTransform.Position.Y / tilemap.TileHeight));
            playerPosition = new Point((int)(playerTransform.Position.X / tilemap.TileWidth),
                (int)(playerTransform.Position.Y / tilemap.TileHeight));

            currentPath = pathfinder.FindPath(nodeMap, enemyPosition, playerPosition); 
            currentPathIndex = 0;

            if (currentPath == null)
            {
                Debug.WriteLine("No path found");
            }
        }
        /// <summary>
        /// Simple state machine for the enemy AI.
        /// </summary>
        public enum EnemyState
        {
            Stunned,
            RangedAttack,
            Follow,
            Attack
        }

        /// <summary>
        /// This method changes the state of the enemy AI.
        /// </summary>
        /// <param name="state"></param>
        protected void ChangeState(EnemyState state)
        {
            CurrentState = state;
        }

        /// <summary>
        /// Initialize the enemy AI with the player, enemy, and tilemap references
        /// </summary>
        public virtual void Initialize()
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
                Debug.WriteLine($"Enemy Object ID: {enemy.GetHashCode()}, Transform Object ID: {enemyTransform.GetHashCode()}");
                CalculateEnemySpawn();
            }
            if (tilemap != null)
            {
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles);
            }      
        }

        /// <summary>
        /// This method is called when the enemy AI is supposed to follow the player. It checks if the enemy can move to the next tile and moves it.
        /// </summary>
        public virtual void FollowPlayer()
        {
            UpdateTarget();
            if (currentPath == null || currentPathIndex >= currentPath.Count)
            {
                Debug.WriteLine("FollowPlayer: No valid path or reached the end of the path.");
                EndTurn();
                return;
            }

            Point nextPoint = currentPath[currentPathIndex];
            Point playerTile = new Point(
                (int)(playerTransform.Position.X / tilemap.TileWidth),
                (int)(playerTransform.Position.Y / tilemap.TileHeight)
            );

            Debug.WriteLine($"FollowPlayer: Next point: {nextPoint}, Player tile: {playerTile}, Current index: {currentPathIndex}");

            // Check if the next tile is the player's tile
            if (IsAdjacentToPlayer())
            {
                Debug.WriteLine("FollowPlayer: Is adjacent, attacking.");
                ChangeState(EnemyState.Attack);
                var playerHealth = player.GetComponent<HealthComponent>();
                playerHealth?.TakeDamage(1);
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles);
                return;
            }

            Vector2 newPosition = new Vector2(nextPoint.X * tilemap.TileWidth, nextPoint.Y * tilemap.TileHeight);

            if (!IsEnemyAtPosition(newPosition))
            {
                // Move to the next tile.             
                enemyTransform.Position = new Vector2((int)newPosition.X, (int)newPosition.Y);
            }
            else
            {
                newPosition = FindValidPosition(enemyTransform.Position);
                if (newPosition != enemyTransform.Position)
                {
                    enemyTransform.Position = new Vector2((int)newPosition.X, (int)newPosition.Y);
                }
            }

            Debug.WriteLine($"FollowPlayer: After move, enemyTransform.Position = {enemyTransform.Position}");

            currentPathIndex++;

            // Check if the path is complete
            if (currentPathIndex >= currentPath.Count)
            {
                Debug.WriteLine("FollowPlayer: Reached the end of the path.");
                currentPath = null;
                currentPathIndex = 0;
            }

            EndTurn();
        }

        protected virtual Vector2 FindValidPosition(Vector2 originalPosition)
        {
            if (currentPath != null && currentPath.Count > 0)
            {
                Point collidingEnemyPoint = new Point(
                    (int)(currentPath[currentPathIndex].X),
                    (int)(currentPath[currentPathIndex].Y)
                );

                // Recalculate path, excluding the colliding enemy's position
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles, collidingEnemyPoint); 
                UpdateTarget(); // Recalculate the path to the player.

                if (currentPath != null && currentPath.Count > 0)
                {
                    // Check if the new path's first step is valid
                    Vector2 newPosition = new Vector2(
                        currentPath[0].X * tilemap.TileWidth,
                        currentPath[0].Y * tilemap.TileHeight
                    );

                    if (!IsEnemyAtPosition(newPosition))
                    {
                        currentPathIndex = 0; // Reset index for the new path
                        return newPosition;
                    }
                }
            }
            return originalPosition; // If no valid new path is found, return the original position.
        }


        /// <summary>
        /// This method is called when the player exits a tile, to recalculate the enemy spawn position.
        /// </summary>
        protected virtual void CalculateEnemySpawn()
            {
                enemyTransform.Position = GetSpawnPosition(playerTransform.Position, 256);
            }

        /// <summary>
        /// This method calculates a spawn position for the enemy, based on the player's position and a minimum distance.
        /// </summary>
        /// <param name="playerPosition"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
            public virtual Vector2 GetSpawnPosition(Vector2 playerPosition, float minDistance)
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
            protected virtual bool IsWalkableTile(int tileX, int tileY)
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
            }
            switch (CurrentState)
            {
                case EnemyState.Follow:
                    Debug.WriteLine("Enemy Turn");
                    FollowPlayer();
                    break;

                case EnemyState.Attack:
                    if (IsAdjacentToPlayer())
                    {
                        DealDamage();
                    }
                    ChangeState(EnemyState.Follow);
                    EndTurn();                 
                    break;

                case EnemyState.Stunned:
                    EndTurn();
                    break;
            }
        }

        public virtual void DealDamage()
        {
            var playerHealth = player.GetComponent<HealthComponent>();
            playerHealth?.TakeDamage(1);
        }

        public void EndTurn()
        {
            isTurn = false;
        }

        public virtual void Stun()
        {
            var enemyAnimation = ObjectManager.Find(GameObject.Tag).GetComponent<AnimationComponent>();
            enemyAnimation.isLooping = true;
            enemyAnimation.PlayAnimation();
            stunnedCounter = 2; // Reset the stunned counter
            ChangeState(EnemyState.Stunned);
        }

        protected bool IsEnemyAtPosition(Vector2 position)
        {
            foreach (var obj in ObjectManager.FindAll())
            {
                if (obj.Tag.StartsWith("enemy"))
                {
                    if (obj == enemy)
                    {
                        continue; // Skip checking against itself.
                    }

                    Transform otherEnemyTransform = obj.GetComponent<Transform>();
                    if (otherEnemyTransform.Position == position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void TakeTurn()
        {
            if (isTurn)
            {
                // Prevent's multiple turns from being taken.
                return;
            }    
        }

        protected virtual bool IsAdjacentToPlayer()
        {
            if (enemy != null && player != null && tilemap != null)
            {
                // Calculate tile positions.
                int enemyTileX = (int)(enemyTransform.Position.X / tilemap.TileWidth);
                int enemyTileY = (int)(enemyTransform.Position.Y / tilemap.TileHeight);
                int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                // Check for adjacency to player (orthogonal and diagonal).
                if (Math.Abs(enemyTileX - playerTileX) <= 1 && Math.Abs(enemyTileY - playerTileY) <= 1)
                {
                    return true;
                }
            }
            return false;
        }
        public void StartTurn()
        {          
            isTurn = true;
        }
    }
}


        