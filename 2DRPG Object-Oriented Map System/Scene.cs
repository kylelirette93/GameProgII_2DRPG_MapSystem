﻿using Microsoft.Xna.Framework;
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

        public GameObject Enemy { get; private set; }
        public GameObject Enemy2 { get; private set; }

        public GameObject Tilemap { get; private set; }

        public GameObject Exit { get; private set; }


        /// <summary>
        /// Initializes the scene with the player and tilemap.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Initialize(MapManager mapManager)
        {
            // 1. Create Tilemap.
            Tilemap = GameObjectFactory.CreateTilemap(mapManager);
            ObjectManager.AddGameObject(Tilemap);

            // 2. Create Player.
            Player = GameObjectFactory.CreatePlayer(mapManager);
            ObjectManager.AddGameObject(Player);

            // 3. Create Enemies.
            Enemy = GameObjectFactory.CreateEnemy(mapManager, "enemy");
            ObjectManager.AddGameObject(Enemy);

            Enemy2 = GameObjectFactory.CreateEnemy(mapManager, "enemy2");
            ObjectManager.AddGameObject(Enemy2);

            // 4. Create Exit.
            Exit = GameObjectFactory.CreateExit(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Exit);

            // 5. Setup Player Exit Event
            Player.GetComponent<PlayerController>().OnExitTile += () => HandleExitTile(mapManager);
            Tilemap.GetComponent<Tilemap>().LastExitTile = Exit.GetComponent<Transform>().Position;

            // 6. Update Object Manager and start turn cycle.
            ObjectManager.UpdateAll();
            TurnManager.StartTurnCycle();
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

            Debug.WriteLine("Before enemy creation.");
            Enemy = GameObjectFactory.CreateEnemy(mapManager, "enemy");
            Enemy2 = GameObjectFactory.CreateEnemy(mapManager, "enemy2");
            Debug.WriteLine("After enemy creation.");

            ObjectManager.AddGameObject(Enemy);
            // Debug.WriteLine("Enemy added.");
            ObjectManager.AddGameObject(Enemy2);
            // Debug.WriteLine("Enemy2 added.");

            // Create the new exit first
            Exit = GameObjectFactory.CreateExit(mapManager, Player.GetComponent<Transform>().Position, 32);
            ObjectManager.AddGameObject(Exit);

            // Set the player's position last
            mapManager.SetPlayerStartPosition(Player, previousExit);

            // Clear the path after the new exit is created
            int tileSize = Tilemap.GetComponent<Tilemap>().TileWidth; 

            Point playerPixelPos = new Point((int)Player.GetComponent<Transform>().Position.X, (int)Player.GetComponent<Transform>().Position.Y);
            Point exitPixelPos = new Point((int)Exit.GetComponent<Transform>().Position.X, (int)Exit.GetComponent<Transform>().Position.Y);

            Point playerTilePos = new Point(playerPixelPos.X / tileSize, playerPixelPos.Y / tileSize);
            Point exitTilePos = new Point(exitPixelPos.X / tileSize, exitPixelPos.Y / tileSize);

            Tilemap.GetComponent<Tilemap>().ClearPathToExit(playerTilePos, exitTilePos);

            TurnManager.TurnQueue.Clear();
            ObjectManager.UpdateAll();
            TurnManager.PopulateQueue();
            Debug.WriteLine("After PopulateQueue.");
            TurnManager.StartTurnCycle();

            // Remove the old exit last
            ObjectManager.RemoveGameObject(previousExit);
        }
    }
}
