using Microsoft.Xna.Framework;
using System.Threading;
using System;

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

        public EnemyAI(string name)
        {
            this._name = name;
        }

        public void Initialize()
        {
            if (player == null)
            {
                player = ObjectManager.Find("player");
                if (player != null)
                {
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
            direction.Normalize();
            enemyTransform.Position += direction * 16f;
            isTurn = false;
            TurnManager.EndTurn();
        }

        private void CalculateEnemySpawn()
        {
            enemyTransform.Position = GetSpawnPosition(playerTransform.Position, 100f);
        }

        public Vector2 GetSpawnPosition(Vector2 playerPosition, float minDistance)
        {
            Random random = new Random();
            Vector2 spawnPosition;

            do
            {
                float x = random.Next(32, 768);
                float y = random.Next(32, 448);
                spawnPosition = new Vector2(x, y);
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
            if (isTurn)
            {
                FollowPlayer();
            }
        }

        public override void TakeTurn()
        {
            isTurn = true;
        }
    }
}
