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
            Debug.WriteLine($"RangedEnemyAI constructor called with name: '{name}'");
        }
        public override void UpdateTarget()
        {
            base.UpdateTarget();
        }       

        public override void Stun()
        {
            base.Stun();
        }

        public override void FollowPlayer()
        {
            base.FollowPlayer();
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
            switch (CurrentState)
            {
                case EnemyState.Follow:
                    Debug.WriteLine("Enemy Turn");
                    FollowPlayer();
                    ChangeState(EnemyState.Scan);
                    break;

                case EnemyState.Scan:
                    Debug.WriteLine("Enemy Turn, Scan State");
                    if (IsInLineOfSight())
                    {
                        FireProjectile();
                    }
                    EndTurn();
                    ChangeState(EnemyState.Follow);               
                    break;
                case EnemyState.Attack:
                    Debug.WriteLine("Enemy Turn, Attack State");
                    if (IsAdjacentToPlayer())
                    {
                        DealDamage();
                    }
                    EndTurn();
                    ChangeState(EnemyState.Follow);
                    break;

                case EnemyState.Stunned:
                    EndTurn();
                    break;
            }
        }

        protected bool IsInLineOfSight()
        {
            if (enemy != null && player != null && tilemap != null)
            {
                int enemyTileX = (int)(enemyTransform.Position.X / tilemap.TileWidth);
                int enemyTileY = (int)(enemyTransform.Position.Y / tilemap.TileHeight);
                int playerTileX = (int)(player.GetComponent<Transform>().Position.X / tilemap.TileWidth);
                int playerTileY = (int)(player.GetComponent<Transform>().Position.Y / tilemap.TileHeight);

                bool inSight = (enemyTileY == playerTileY && enemyTileX != playerTileX) ||
                               (enemyTileX == playerTileX && enemyTileY != playerTileY);

                Debug.WriteLine($"Enemy Tile: ({enemyTileX}, {enemyTileY}), Player Tile: ({playerTileX}, {playerTileY}), In Sight: {inSight}");

                return inSight;
            }
            return false;
        }


        public void FireProjectile()
        {
            Debug.WriteLine("FireProjectile() called.");

                CreateProjectile();
                if (projectile == null)
                {
                    FollowPlayer();
                    EndTurn();
                }
            
        }

        public void CreateProjectile()
        {
            GameObject projectile = GameObjectFactory.CreateProjectile(enemy.GetComponent<Transform>().Position);
            ObjectManager.AddGameObject(projectile);

            Transform projectileTransform = projectile.GetComponent<Transform>();
            Vector2 projectileDirection = playerTransform.Position - enemyTransform.Position;
            projectileDirection.Normalize();
            projectile.GetComponent<ProjectileComponent>().Direction = projectileDirection;
        }

        public override void DealDamage()
        {
            base.DealDamage();
        }
    }
}
