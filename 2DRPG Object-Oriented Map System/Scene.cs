using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Initializes the scene with the player and tilemap.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Initialize(MapManager mapManager)
        {
            // Create the player and tilemap.
            Tilemap = GameObjectFactory.CreateTilemap(mapManager);
            Player = GameObjectFactory.CreatePlayer(mapManager);
            Enemy = GameObjectFactory.CreateEnemy(mapManager, "enemy");
            Enemy2 = GameObjectFactory.CreateEnemy(mapManager, "enemy2");
            // Wrapper function to pass map manager argument to the event handler.
            Player.GetComponent<PlayerController>().OnExitTile += () => HandleExitTile(mapManager);

            // Add the player and tilemap to the game manager.
            ObjectManager.AddGameObject(Tilemap);
            ObjectManager.AddGameObject(Player);
            ObjectManager.AddGameObject(Enemy);
            ObjectManager.AddGameObject(Enemy2);
            TurnManager.StartTurnCycle();
        }

        private void HandleExitTile(MapManager mapManager)
        {
            Transition(mapManager);
        }

        private void Transition(MapManager mapManager)
        {
            // Transition to next scene, replacing tilemap.
            mapManager.NextMap();
            Tilemap.RemoveComponent<Tilemap>();
            Tilemap.AddComponent(mapManager.CreateMap());
            mapManager.SetPlayerStartPosition(Player);
        }
}
}
