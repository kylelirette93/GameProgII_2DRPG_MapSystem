using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Base class for enemy AI. Contains pathfinding logic and state machine for enemy AI.
    /// </summary>
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
        /// </summary>
        /// <param name="name"></param>
        public BaseEnemyAI(string name)
        {
            // Unique ID for turn manager.
            Id = Guid.NewGuid().ToString();
            // Name of the enemy ai for object manager.
            _name = name;
            stunnedCounter = 0;
            pathfinder = new Pathfinder();
        }

        /// <summary>
        /// This method is responsible for finding a path to the player and updating the current path.
        /// </summary>
        public virtual void UpdateTarget()
        {
            if (tilemap == null || playerTransform == null || enemyTransform == null)
            {
                //Debug.WriteLine("Can't find target, somethings null!");
                return;
            }

            // Convert tile coordinates to point coordinates.
            enemyPosition = new Point((int)(enemyTransform.Position.X / tilemap.TileWidth), (int)(enemyTransform.Position.Y / tilemap.TileHeight));
            playerPosition = new Point((int)(playerTransform.Position.X / tilemap.TileWidth), (int)(playerTransform.Position.Y / tilemap.TileHeight));

            // Find a path based on the enemy and player positions.
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
        public enum EnemyState { Stunned, RangedAttack, Follow, Attack }

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
                //Debug.WriteLine($"Enemy Object ID: {enemy.GetHashCode()}, Transform Object ID: {enemyTransform.GetHashCode()}");
                CalculateEnemySpawn();
            }
            if (tilemap != null)
            {
                // Build the node map for pathfinding based on the tilemap.
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
                EndTurn();
                return;
            }

            Point nextPoint = currentPath[currentPathIndex];
            Vector2 newPosition = new Vector2(nextPoint.X * tilemap.TileWidth, nextPoint.Y * tilemap.TileHeight);

            // Boundary Check BEFORE assigning new position
            if (newPosition.X < 0 || newPosition.X >= tilemap.TileWidth * tilemap.MapWidth ||
                newPosition.Y < 0 || newPosition.Y >= tilemap.TileHeight * tilemap.MapHeight)
            {
                // Handle out-of-bounds situation (e.g., recalculate path, random move)
                newPosition = FindValidPosition(enemyTransform.Position); // Try to find another position
                if (newPosition == enemyTransform.Position)
                {
                    // if no valid position is found, stop movement.
                    EndTurn();
                    return;
                }
            }

            if (IsAdjacentToPlayer())
            {
                ChangeState(EnemyState.Attack);
                DealDamage();
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles);
                return;
            }

            if (!IsEnemyAtPosition(newPosition))
            {
                enemyTransform.Position = newPosition;
                ClampPosition();
            }
            else
            {
                Vector2 validPosition = FindValidPosition(enemyTransform.Position);
                if (validPosition != enemyTransform.Position)
                {
                    enemyTransform.Position = validPosition;
                    ClampPosition();
                }
                else
                {
                    // If no valid position is found, recalculate path, random move, or wait.
                    //Example: Recalculate path
                    UpdateTarget();
                    if (currentPath == null || currentPath.Count == 0)
                    {
                        EndTurn();
                        return;
                    }
                    nextPoint = currentPath[0];
                    newPosition = new Vector2(nextPoint.X * tilemap.TileWidth, nextPoint.Y * tilemap.TileHeight);
                    if (newPosition.X < 0 || newPosition.X >= tilemap.TileWidth * tilemap.MapWidth ||
                        newPosition.Y < 0 || newPosition.Y >= tilemap.TileHeight * tilemap.MapHeight)
                    {
                        EndTurn();
                        return;
                    }
                    enemyTransform.Position = newPosition;
                    ClampPosition();

                    currentPathIndex = 1;
                }
            }

            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                currentPath = null;
                currentPathIndex = 0;
            }

            EndTurn();
        }

        /// <summary>
        /// This method checks if a new position is valid and updates pathfinding with enemy point.
        /// </summary>
        /// <param name="originalPosition"></param>
        /// <returns></returns>
        protected virtual Vector2 FindValidPosition(Vector2 originalPosition)
        {
            if (currentPath != null && currentPath.Count > 0)
            {
                Point collidingEnemyPoint = currentPath[currentPathIndex];
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles, collidingEnemyPoint);
                UpdateTarget();

                if (currentPath != null && currentPath.Count > 0)
                {
                    Vector2 newPosition = new Vector2(currentPath[0].X * tilemap.TileWidth, currentPath[0].Y * tilemap.TileHeight);

                    if (!IsEnemyAtPosition(newPosition))
                    {
                        currentPathIndex = 0;
                        return newPosition;
                    }
                }
            }
            return originalPosition;
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
                int tileX = _random.Next(1, 768 / 32);
                int tileY = _random.Next(1, 448 / 32);
                spawnPosition = new Vector2(tileX * 32, tileY * 32);

                if (Vector2.Distance(spawnPosition, playerPosition) >= minDistance && IsWalkableTile(tileX, tileY))
                {
                    validPosition = true;
                }
            }
            ClampPosition();
            return spawnPosition;
        }

        /// <summary>
        /// This method clamp's the enemy's position to the tilemap bounds.
        /// </summary>
        public void ClampPosition()
        {
            Vector2 currentPosition = enemyTransform.Position;

            float minX = 0;
            float minY = 0;
            float maxX = tilemap.Tiles.GetLength(0) * tilemap.TileWidth;
            float maxY = tilemap.Tiles.GetLength(1) * tilemap.TileHeight;

            // Check if the enemy is outside the map bounds.
            if (currentPosition.X < minX || currentPosition.X > maxX ||
                currentPosition.Y < minY || currentPosition.Y > maxY)
            {
                TurnManager.Instance.RemoveTurnTaker(this);
                enemy.Destroy();
                Debug.WriteLine($"Enemy {_name} destroyed due to being out of map bounds.");
                return; // Exit the method to prevent further processing.
            }

            // Clamp x and y coordinates to the map bounds.
            currentPosition.X = Math.Clamp(currentPosition.X, minX, maxX);
            currentPosition.Y = Math.Clamp(currentPosition.Y, minY, maxY);

            // Nudge enemy's position if it's on a wall.
            if (currentPosition.X == minX || currentPosition.X == maxX)
            {
                currentPosition.X += (currentPosition.X == minX) ? 1 : -1;
            }

            if (currentPosition.Y == minY || currentPosition.Y == maxY)
            {
                currentPosition.Y += (currentPosition.Y == minY) ? 1 : -1;
            }

            enemyTransform.Position = currentPosition;
        }

        /// <summary>
        /// This method checks if a tile is walkable.
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        protected virtual bool IsWalkableTile(int tileX, int tileY)
        {
            if (tilemap != null && tileX >= 0 && tileX < tilemap.Tiles.GetLength(0) && tileY >= 0 && tileY < tilemap.Tiles.GetLength(1))
            {
                return tilemap.Tiles[tileX, tileY].IsWalkable;
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

            if (!isTurn) return;

            if (CurrentState == EnemyState.Stunned && stunnedCounter > 0)
            {
                stunnedCounter--;
                if (stunnedCounter == 0)
                {
                    CurrentState = EnemyState.Follow;
                }
            }

            switch (CurrentState)
            {
                case EnemyState.Follow:
                    FollowPlayer();
                    break;
                case EnemyState.Attack:
                    if (IsAdjacentToPlayer()) DealDamage();
                    ChangeState(EnemyState.Follow);
                    EndTurn();
                    break;
                case EnemyState.Stunned:
                    EndTurn();
                    break;
            }
        }

        /// <summary>
        /// This method retrieves the player's health component and deals damage to the player.
        /// </summary>
        public virtual void DealDamage()
        {
            player.GetComponent<HealthComponent>()?.TakeDamage(1);
        }

        public virtual void EndTurn()
        {
            isTurn = false;
        }
        /// <summary>
        /// This method stuns the enemy for a set number of turns.
        /// </summary>
        public virtual void Stun()
        {
            stunnedCounter = 2;
            ChangeState(EnemyState.Stunned);
        }

        /// <summary>
        /// This method checks if an enemy is at a given position. Useful for pathfinding.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool IsEnemyAtPosition(Vector2 position)
        {
            foreach (var obj in ObjectManager.FindAll())
            {
                if (obj.Tag.StartsWith("enemy") && obj != enemy && obj.GetComponent<Transform>().Position == position)
                {
                    return true;
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

        /// <summary>
        /// Check's if an enemy is adjacent to the player.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsAdjacentToPlayer()
        {
            if (enemy == null || player == null || tilemap == null) return false;

            int enemyTileX = (int)(enemyTransform.Position.X / tilemap.TileWidth);
            int enemyTileY = (int)(enemyTransform.Position.Y / tilemap.TileHeight);
            int playerTileX = (int)(playerTransform.Position.X / tilemap.TileWidth);
            int playerTileY = (int)(playerTransform.Position.Y / tilemap.TileHeight);

            return (Math.Abs(enemyTileX - playerTileX) == 1 && Math.Abs(enemyTileY - playerTileY) == 0) ||
                   (Math.Abs(enemyTileX - playerTileX) == 0 && Math.Abs(enemyTileY - playerTileY) == 1);
        }

        public void StartTurn()
        {          
            isTurn = true;
        }
    }
}


        