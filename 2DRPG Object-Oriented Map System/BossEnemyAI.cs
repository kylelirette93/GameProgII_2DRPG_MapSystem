using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class BossEnemyAI : RangedEnemyAI
    {
        private int nextAction;
        private int chargeDistance = 3;
        public BossEnemyAI(string name) : base(name)
        {
            nextAction = new Random().Next(4);
        }

        public override void Update()
        {
            base.Update();
            if (!isTurn)
            {
                return;
            }
            HandleBossActions();
        }

        private void HandleBossActions()
        {
            switch (nextAction)
            {
                case 0: // Nothing
                    Debug.WriteLine("Boss: Doing Nothing");
                    EndTurn();
                    break;
                case 1: // Move
                    Debug.WriteLine("Boss: Moving");
                    MoveTowardsPlayer();
                    EndTurn();
                    break;
                case 2: // Shoot
                    Debug.WriteLine("Boss: Shooting");
                    FireProjectile();
                    EndTurn();
                    break;
                case 3: // Charge
                    Debug.WriteLine("Boss: Charging");
                    ChargeTowardsPlayer();
                    EndTurn();
                    break;
            }
            nextAction = new Random().Next(4);
            EndTurn();
        }

        private void MoveTowardsPlayer()
        {
            FollowPlayer();
        }
        private void ChargeTowardsPlayer()
        {
            Vector2 chargeDirection = playerTransform.Position - enemyTransform.Position;
            chargeDirection.Normalize();

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
                    break;
                }
            }
        }
    }
}
