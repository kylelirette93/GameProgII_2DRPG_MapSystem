using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Scene class is responsible for Initializing the game objects in the scene and handling transitions between scenes.
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Read only properties for player and tilemap, the property can only be modified from within the class. 
        /// </summary>

        public GameObject Player { get; private set; }     

        public GameObject Tilemap { get; private set; }

        public GameObject Exit { get; private set; }

        public GameObject TurnArrow { get; private set; }

        public GameObject Item { get; private set; }

        public GameObject Boss { get; private set; }

        private Random random = new Random();
        


        /// <summary>
        /// Initializes the scene with the player and tilemap.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Initialize(MapManager mapManager)
        {
            Tilemap = GameObjectFactory.CreateTilemap(mapManager);
            ObjectManager.AddGameObject(Tilemap);

            Player = GameObjectFactory.CreatePlayer(mapManager);
            ObjectManager.AddGameObject(Player);

            Item = GameObjectFactory.CreateRandomItem(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Item);

            SpawnRandomEnemies(mapManager);

            Exit = GameObjectFactory.CreateExit(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Exit);

            Player.GetComponent<PlayerController>().OnExitTile += () => HandleExitTile(mapManager);
            Tilemap.GetComponent<Tilemap>().LastExitTile = Exit.GetComponent<Transform>().Position;

            TurnArrow = GameObjectFactory.CreateTurnArrow();
            ObjectManager.AddGameObject(TurnArrow);

            TurnManager.Instance.AddTurnTaker(Player.GetComponent<PlayerController>());
            AddEnemyTurns();
        }

        public void Update(GameTime gameTime)
        {
            TurnManager.Instance.UpdateTurn(gameTime);

            if (Player.GetComponent<PlayerController>().IsTurn && !Player.GetComponent<PlayerController>().IsShooting)  
            TurnArrow.GetComponent<Transform>().Position = Player.GetComponent<Transform>().Position + new Vector2(1, -32);
            else
            {
                TurnArrow.GetComponent<Transform>().Position = new Vector2(-100, -100);
            }
        }

        private void HandleExitTile(MapManager mapManager)
        {
            Transition(mapManager);
        }

        private void Transition(MapManager mapManager)
        {
            GameObject previousExit = Exit;
            mapManager.NextMap();
            Tilemap.RemoveComponent<Tilemap>();
            Tilemap.AddComponent(mapManager.CreateMap());
            Tilemap.GetComponent<Tilemap>().LastExitTile = Exit.GetComponent<Transform>().Position;

            Item = GameObjectFactory.CreateRandomItem(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Item);

            SpawnRandomEnemies(mapManager);

            if (mapManager.CurrentLevelIndex > 3)
            {
                Boss = GameObjectFactory.CreateBossEnemy(mapManager);
            }

                Exit = GameObjectFactory.CreateExit(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Exit);

            mapManager.SetPlayerStartPosition(Player, previousExit);

            int tileSize = Tilemap.GetComponent<Tilemap>().TileWidth; 

            Point playerPixelPos = new Point((int)Player.GetComponent<Transform>().Position.X, (int)Player.GetComponent<Transform>().Position.Y);
            Point exitPixelPos = new Point((int)Exit.GetComponent<Transform>().Position.X, (int)Exit.GetComponent<Transform>().Position.Y);

            Point playerTilePos = new Point(playerPixelPos.X / tileSize, playerPixelPos.Y / tileSize);
            Point exitTilePos = new Point(exitPixelPos.X / tileSize, exitPixelPos.Y / tileSize);

            Tilemap.GetComponent<Tilemap>().ClearPathToExit(playerTilePos, exitTilePos);


            // Remove the old exit last
            ObjectManager.RemoveGameObject(previousExit);
            AddEnemyTurns();
        }

        private void SpawnRandomEnemies(MapManager mapManager)
        {
            int enemyCount = random.Next(2, 4);
            for (int i = 0; i < enemyCount; i++)
            {
                int enemyType = random.Next(3);
                string enemyName;
                string textureName;

                switch (enemyType)
                {
                    case 0:
                        enemyName = $"enemy_{Guid.NewGuid()}"; 
                        textureName = "enemy";
                        ObjectManager.AddGameObject(GameObjectFactory.CreateEnemy(mapManager, enemyName, textureName));
                        Debug.WriteLine($"Spawned {enemyName} with BaseEnemyAI (or MeleeEnemyAI).");
                        break;
                    case 1:
                        enemyName = $"enemy2_{Guid.NewGuid()}"; 
                        textureName = "ranged_enemy";
                        ObjectManager.AddGameObject(GameObjectFactory.CreateRangedEnemy(mapManager, enemyName, textureName));
                        Debug.WriteLine($"Spawned {enemyName} with RangedEnemyAI.");
                        break;
                    case 2:
                        enemyName = $"enemy3_{Guid.NewGuid()}";
                        textureName = "ghost_enemy";
                        ObjectManager.AddGameObject(GameObjectFactory.CreateGhostEnemy(mapManager, enemyName, textureName));
                        Debug.WriteLine($"Spawned {enemyName} with GhostEnemyAI.");
                        break;
                    default:
                        enemyName = $"enemy_{Guid.NewGuid()}"; 
                        textureName = "enemy";
                        ObjectManager.AddGameObject(GameObjectFactory.CreateEnemy(mapManager, enemyName, textureName));
                        Debug.WriteLine($"Spawned {enemyName} with BaseEnemyAI (or MeleeEnemyAI).");
                        break;
                }
                // Debug.WriteLine($"Enemy type: {enemyType}");
            }
        }

        /// <summary>
        /// This method is responsible for clearing the scene when game over happens.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Clear(MapManager mapManager)
        {
            // Clear the scene.
            if (Player != null && Player.GetComponent<PlayerController>() != null)
            {
                Player.GetComponent<PlayerController>().OnExitTile -= () => HandleExitTile(mapManager);
            }
            Player = null;
            Tilemap = null;
            Exit = null;
            TurnArrow = null;
            Item = null;
            ObjectManager.GameObjects.Clear();
        }

        /// <summary>
        /// This method add's each random enemy to the turn cycle.
        /// </summary>
        private void AddEnemyTurns()
        {
            foreach (GameObject gameObject in ObjectManager.GameObjects)
            {
                if (gameObject.GetComponent<MeleeEnemyAI>() != null)
                {
                    TurnManager.Instance.AddTurnTaker(gameObject.GetComponent<MeleeEnemyAI>());
                }
                else if (gameObject.GetComponent<RangedEnemyAI>() != null)
                {
                    TurnManager.Instance.AddTurnTaker(gameObject.GetComponent<RangedEnemyAI>());
                }
                else if (gameObject.GetComponent<GhostEnemyAI>() != null)
                {
                    TurnManager.Instance.AddTurnTaker(gameObject.GetComponent<GhostEnemyAI>());
                }
            }
        }
    }
}
