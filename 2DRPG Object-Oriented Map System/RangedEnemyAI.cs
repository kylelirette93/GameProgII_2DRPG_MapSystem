using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class RangedEnemyAI : BaseEnemyAI
    {
        GameObject projectile;
        public RangedEnemyAI(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
            //Debug.WriteLine($"RangedEnemyAI constructor called with name: '{name}'");
        }       

        public override void Update()
        {
            if (player != null)
            {
                playerTransform.Position = player.GetComponent<Transform>().Position;
            }
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
            bool lineOfSight = IsInLineOfSight();
            bool adjacent = IsAdjacentToPlayer();
            switch (CurrentState)
            {
                case EnemyState.Follow:
                    Debug.WriteLine("Enemy Turn");
                    if (IsAdjacentToPlayer())
                    {
                        ChangeState(EnemyState.Attack);
                    }
                    else if (IsInLineOfSight())
                    {
                        ChangeState(EnemyState.RangedAttack);
                    }
                    else
                    {
                        FollowPlayer();
                        EndTurn();
                    }
                    
                    break;

                case EnemyState.RangedAttack:
                    if (projectile == null)
                    {
                        FireProjectile();
                    }
                    break;
                case EnemyState.Attack:
                    Debug.WriteLine("Enemy Turn, Attack State");
                    DealDamage();
                    EndTurn();
                    break;

                case EnemyState.Stunned:
                    EndTurn();
                    break;
            }
        }

        private bool IsInLineOfSight()
        {
            if (enemy != null && player != null && tilemap != null)
            {
                int enemyTileX = (int)(enemyTransform.Position.X / tilemap.TileWidth);
                int enemyTileY = (int)(enemyTransform.Position.Y / tilemap.TileHeight);
                int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                // Check if they are in the same row or column
                if (enemyTileY == playerTileY && enemyTileX != playerTileX)
                {
                    // Horizontal line of sight.
                    int startX = Math.Min(enemyTileX, playerTileX) + 1;
                    int endX = Math.Max(enemyTileX, playerTileX);

                    for (int x = startX; x < endX; x++)
                    {
                        if (!tilemap.Tiles[x, enemyTileY].IsWalkable)
                        {
                            return false; 
                        }
                    }
                    return true; 
                }
                else if (enemyTileX == playerTileX && enemyTileY != playerTileY)
                {
                    // Vertical line of sight
                    int startY = Math.Min(enemyTileY, playerTileY) + 1;
                    int endY = Math.Max(enemyTileY, playerTileY);

                    for (int y = startY; y < endY; y++)
                    {
                        if (!tilemap.Tiles[enemyTileX, y].IsWalkable)
                        {
                            return false; // Obstacle in the way.
                        }
                    }
                    return true; // Line of sight clear.
                }
                else
                {
                    return false; // Not in the same row or column.
                }
            }
            return false;
        }

        public void FireProjectile()
        {
            //Debug.WriteLine("FireProjectile() called.")
            projectile = CreateProjectile();
        }

        public void EndTurn()
        {
            projectile = null;
            base.EndTurn(); 
            ChangeState(EnemyState.Follow);
        }

        public GameObject CreateProjectile()
        {
            GameObject projectile = GameObjectFactory.CreateProjectile(enemy.GetComponent<Transform>().Position);
            ObjectManager.AddGameObject(projectile);

            Transform projectileTransform = projectile.GetComponent<Transform>();
            Vector2 projectileDirection = playerTransform.Position - enemyTransform.Position;
            projectileDirection.Normalize();
            //Debug.WriteLine($"Projectile direction + {projectileDirection}");
            projectile.GetComponent<ProjectileComponent>().Direction = projectileDirection;
            projectile.GetComponent<ProjectileComponent>().EnemyTag = enemy.Tag;
            return projectile;
        }
    }
}
