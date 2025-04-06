using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class BossEnemyAI : RangedEnemyAI
    {
        private int nextAction;
        private int currentAction;
        private int chargeDistance = 3;
        public BossActions CurrentAction;
        DisplayIcon displayIcon;

        Stopwatch timer = new Stopwatch();
        bool waitingForNextTurn = false;
        private int turnDelay = 5000;

        public BossEnemyAI(string name) : base(name)
        {
            nextAction = new Random().Next(4);
        }

        public enum BossActions
        {
            Idle,
            Move,
            Shoot,
            Charge,
            Stunned,
            Follow,
            Attack
        }

        public override void Update()
        {
            stunnedCounter = 0;
            base.Update();
            if (!isTurn)
            {
                return;
            }
            if (waitingForNextTurn)
            {
                if (timer.ElapsedMilliseconds > turnDelay)
                {
                    waitingForNextTurn = false;
                    nextAction = new Random().Next(4);
                    DisplayNextAction();
                    EndTurn();
                }
            }
            else
            {
                // Add a check to prevent repeat calls, if needed.
                HandleBossActions();
                waitingForNextTurn = true;
                timer.Restart();
            }
        }

        private void DisplayNextAction()
        {
            displayIcon = ObjectManager.Find("Boss").GetComponent<DisplayIcon>();
            switch (nextAction)
            {
                case (int)BossActions.Idle:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_idle_icon"));
                    break;
                case (int)BossActions.Move:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_move_icon"));
                    break;
                case (int)BossActions.Shoot:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_shoot_icon"));
                    break;
                case (int)BossActions.Charge:
                    displayIcon.SetIcon(AssetManager.GetTexture("boss_charge_icon"));
                    break;
            }
        }
        private void HandleBossActions()
        {
            CurrentAction = (BossActions)nextAction; // Use the stored current action

            switch (CurrentAction)
            {
                case BossActions.Idle: // Idle
                    break;
                case BossActions.Move: // Move
                    MoveTowardsPlayer();
                    break;
                case BossActions.Shoot: // Shoot
                    FireProjectile();
                    break;
                case BossActions.Charge: // Charge
                    ChargeTowardsPlayer();
                    break;
            }
        }

        private void MoveTowardsPlayer()
        {
            FollowPlayer();
        }
        private void ChargeTowardsPlayer()
        {
            Vector2 chargeDirection = GetClosestDirection();
            for (int i = 0; i < chargeDistance; i++)
            {
                Vector2 nextPosition = enemyTransform.Position + chargeDirection * tilemap.TileWidth;

                int nextTileX = (int)(nextPosition.X / tilemap.TileWidth);
                int nextTileY = (int)(nextPosition.Y / tilemap.TileHeight);

                if (!tilemap.Tiles[nextTileX, nextTileY].IsWalkable)
                {
                    break;
                }

                enemyTransform.Position = nextPosition;

                if (IsAdjacentToPlayer())
                {
                    DealDamage();
                    SoundManager.PlaySound("charge");
                    break;
                }
            }
            ShakeMap();
           
        }

        private Vector2 GetClosestDirection()
        {
            Vector2 direction = playerTransform.Position - enemyTransform.Position;
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

        private void ShakeMap()
        {
            Tilemap tilemap = ObjectManager.Find("tilemap").GetComponent<Tilemap>();
            if (tilemap != null)
            {
                tilemap.Shake();
            }
        }
    }
}
