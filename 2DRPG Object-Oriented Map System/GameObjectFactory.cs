using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Game Object Factory is a factory class with a static factory method pattern. It encapsulates object creation.
    /// </summary>
    public static class GameObjectFactory
    {
        public static GameObject CreatePlayer(MapManager mapManager)
        {
            GameObject player = new GameObject("player");
            player.AddComponent(new Transform());
            player.AddComponent(new Sprite(SpriteManager.GetTexture("player")));
            player.AddComponent(new Collider(player.GetComponent<Sprite>().SpriteBounds));
            player.AddComponent(new PlayerController());
            player.AddComponent(new HealthComponent(100));
            return player;
        }
        public static GameObject CreateTilemap(MapManager mapManager)
        {
            GameObject tilemapObject = new GameObject("tilemap");
            Tilemap tilemap = mapManager.CreateMap();
            tilemapObject.AddComponent(tilemap);
            tilemapObject.AddComponent(new Transform());
            return tilemapObject;
        }

        public static GameObject CreateEnemy(MapManager mapManager, string name)
        {
            GameObject enemy = new GameObject(name);
            enemy.AddComponent(new Transform());
            enemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn(name);
            enemy.AddComponent(new Sprite(SpriteManager.GetTexture("enemy")));
            enemy.AddComponent(new Collider(enemy.GetComponent<Sprite>().SpriteBounds));
            enemy.AddComponent(new EnemyAI(name));
            enemy.AddComponent(new HealthComponent(40));
            Debug.WriteLine("Created enemy: " + name + "Turn Component: " + enemy.GetComponent<EnemyAI>());
            return enemy;
        }
        // To use for procedural generation.
        public static GameObject GenerateTilemap()
        {
            Tilemap tilemap = new Tilemap();
            GameObject tilemapObject = new GameObject("tilemap");
            tilemapObject.AddComponent(tilemap);
            tilemapObject.AddComponent(new Transform());
            tilemap.GenerateProceduralMap(15, 10);
            return tilemapObject;
        }

        public static GameObject CreateExit(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
            Vector2 exitPosition = mapManager.CurrentMap.FindExitSpawn(playerPosition, minDistance);
            GameObject exit = new GameObject("exit");
            exit.AddComponent(new Transform());
            exit.GetComponent<Transform>().Position = exitPosition;
            exit.AddComponent(new Sprite(SpriteManager.GetTexture("exit_tile")));
            exit.AddComponent(new Collider(exit.GetComponent<Sprite>().SpriteBounds));
            return exit;
        }
    }
}
