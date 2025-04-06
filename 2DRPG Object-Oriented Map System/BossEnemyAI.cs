using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The Boss Enemy AI class is responsible for handling the boss enemy's AI. It predicts and picks a random action, such as idle, move, shoot, or charge.
    /// </summary>
    public class BossEnemyAI : RangedEnemyAI
    {
        private int nextAction;
        private int currentAction;
        private int chargeDistance = 3;
        public BossActions CurrentAction;
        DisplayIcon displayIcon;

        Stopwatch timer = new Stopwatch();
        bool waitingForNextTurn = false;
        private int turnDelay = 500;
        bool hasTakenAction = false;

        // Pathfinding for boss.
        GameObject boss;
        Transform bossTransform;
        Point bossPosition;

        // Random number generator for boss actions.
        private static readonly Random random = new Random();

        public BossEnemyAI(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
            nextAction = new Random().Next(4);
            name = "Boss";
        }

        /// <summary>
        /// States for the boss actions.
        /// </summary>
        public enum BossActions
        {
            Idle,
            Move,
            Shoot,
            Charge,
        }

        public override void Initialize()
        {
            base.Initialize();
            boss = ObjectManager.Find("Boss");
            if (boss != null)
            {
                bossTransform = boss.GetComponent<Transform>();
            }
            else
            {
                //Debug.WriteLine("Boss not found.");
            }
        }

        public override void UpdateTarget()
        {
            if (tilemap == null || playerTransform == null || bossTransform == null || enemyTransform == null)
            {
                //Debug.WriteLine("Can't find target, somethings null!");
                return;
            }

            bossPosition = new Point((int)(bossTransform.Position.X / tilemap.TileWidth), (int)(bossTransform.Position.Y / tilemap.TileHeight));
            playerPosition = new Point((int)(playerTransform.Position.X / tilemap.TileWidth), (int)(playerTransform.Position.Y / tilemap.TileHeight));

            currentPath = pathfinder.FindPath(nodeMap, bossPosition, playerPosition);
            currentPathIndex = 0;

            if (currentPath == null)
            {
                //Debug.WriteLine("No path found");
            }
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
            bool lineOfSight = IsInLineOfSight();
            bool adjacent = IsAdjacentToPlayer();
            if (!waitingForNextTurn)
            {
                hasTakenAction = false;
                HandleBossActions();
                waitingForNextTurn = true;
                timer.Restart();
            }
            else
            {
                if (timer.ElapsedMilliseconds > turnDelay)
                {
                    waitingForNextTurn = false;
                    nextAction = new Random().Next(4);
                    DisplayNextAction();
                    EndTurn(); // End turn after the delay.
                }
            }
        }

        public override void EndTurn()
        {
            if (hasTakenAction)
            {
                base.EndTurn();
            }
        }

        /// <summary>
        /// This method displays an icon for the next action the boss will take.
        /// </summary>
        private void DisplayNextAction()
        {
            displayIcon = ObjectManager.Find("Boss").GetComponent<DisplayIcon>();
            switch (nextAction)
            {
                case (int)BossActions.Idle:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_idle_icon"), turnDelay);
                    break;
                case (int)BossActions.Move:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_move_icon"), turnDelay);
                    break;
                case (int)BossActions.Shoot:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_shoot_icon"), turnDelay);
                    break;
                case (int)BossActions.Charge:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_charge_icon"), turnDelay);
                    break;
            }
        }

        private void HandleBossActions()
        {
            // Use the stored action.
            CurrentAction = (BossActions)nextAction;
            //Debug.WriteLine($"Boss chose action: {CurrentAction}");

            switch (CurrentAction)
            {
                case BossActions.Idle:
                    hasTakenAction = true;
                    EndTurn();
                    break;
                case BossActions.Move:
                    MoveTowardsPlayer();
                    break;
                case BossActions.Shoot:
                    if (IsInLineOfSight())
                    {
                        if (projectile == null)
                        {
                            FireProjectile();
                        }
                        EndTurn();
                    }
                    else
                    {
                        FollowPlayer();
                        EndTurn();
                    }
                    break;
                case BossActions.Charge:
                    ChargeTowardsPlayer();
                    break;
            }
        }

        private void MoveTowardsPlayer()
        {
            FollowPlayer();
        }

        public override void FollowPlayer()
        {
            UpdateTarget();
            if (currentPath == null || currentPathIndex >= currentPath.Count)
            {
                EndTurn();
                return;
            }

            if (IsAdjacentToPlayer())
            {
                DealDamage();
                nodeMap = pathfinder.BuildNodeMap(tilemap.Tiles);
                return;
            }

            Point nextPoint = currentPath[currentPathIndex];
            Vector2 newPosition = new Vector2(nextPoint.X * tilemap.TileWidth, nextPoint.Y * tilemap.TileHeight);

            bossTransform.Position = newPosition;
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                currentPath = null;
                currentPathIndex = 0;
            }
            hasTakenAction = true;
        }

        private void ChargeTowardsPlayer()
        {
            if (bossTransform == null || playerTransform == null || tilemap == null)
            {
                //Debug.WriteLine("Can't charge, somethings null!");
                return;
            }
            Vector2 chargeDirection = GetClosestDirection();
            for (int i = 0; i < chargeDistance; i++)
            {
                Vector2 nextPosition = bossTransform.Position + chargeDirection * tilemap.TileWidth;

                int nextTileX = (int)(nextPosition.X / tilemap.TileWidth);
                int nextTileY = (int)(nextPosition.Y / tilemap.TileHeight);

                if (!tilemap.Tiles[nextTileX, nextTileY].IsWalkable)
                {
                    break;
                }

                bossTransform.Position = nextPosition;

                if (IsAdjacentToPlayer())
                {
                    DealDamage();
                    SoundManager.PlaySound("charge");
                    break;
                }
            }
            //ShakeMap();
            hasTakenAction = true;
        }

        private Vector2 GetClosestDirection()
        {
            if (playerTransform == null || bossTransform == null)
            {
                return Vector2.Zero;
            }
            Vector2 direction = playerTransform.Position - bossTransform.Position;
            direction.Normalize();

            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                return new Vector2(Math.Sign(direction.X), 0);
            }
            else
            {
                return new Vector2(0, Math.Sign(direction.Y));
            }
        }

        public override GameObject CreateProjectile()
        {
            if (boss == null || player == null || playerTransform == null || bossTransform == null)
            {
                Debug.WriteLine("Can't create projectile, somethings null!");
                return null;
            }
            GameObject projectile = GameObjectFactory.CreateProjectile(boss.GetComponent<Transform>().Position);
            ObjectManager.AddGameObject(projectile);

            Transform projectileTransform = projectile.GetComponent<Transform>();
            Vector2 projectileDirection = playerTransform.Position - bossTransform.Position;
            projectileDirection.Normalize();
            //Debug.WriteLine($"Projectile direction + {projectileDirection}");
            projectile.GetComponent<ProjectileComponent>().Direction = projectileDirection;
            projectile.GetComponent<ProjectileComponent>().EnemyTag = boss.Tag;
            hasTakenAction = true;
            return projectile;
        }
    }
}