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
        public GameObject Player { get; set; }
        public GameObject Tilemap { get; set; }

        /// <summary>
        /// Initializes the scene with the player and tilemap.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Initialize(MapManager mapManager)
        {
            // Create the player and tilemap.
            Tilemap = GameObjectFactory.CreateTilemap(mapManager);
            Player = GameObjectFactory.CreatePlayer(mapManager);
            // Wrapper function to pass map manager argument to the event handler.
            Player.GetComponent<PlayerController>().OnExitTile += () => HandleExitTile(mapManager);

            // Add the player and tilemap to the game manager.
            ObjectManager.AddGameObject(Tilemap);
            ObjectManager.AddGameObject(Player);
        }

        private void HandleExitTile(MapManager mapManager)
        {
            Transition(mapManager);
        }
        /// <summary>
        /// Handles transitioning to the next scene.
        /// </summary>
        /// <param name="mapManager"></param>
        public void Transition(MapManager mapManager)
        {
            // Transition to next scene, replacing tilemap.
            mapManager.NextMap();
            Tilemap.RemoveComponent<Tilemap>();
            Tilemap.AddComponent(mapManager.CreateMap());
            mapManager.SetPlayerStartPosition(Player);
        }
}
}
