using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            player.GetComponent<Transform>().Position = mapManager.SpawnPoint;
            player.AddComponent(new Sprite(SpriteManager.GetTexture("player")));
            player.AddComponent(new Collider(player.GetComponent<Sprite>().SpriteBounds));
            player.AddComponent(new PlayerController());
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
    }
}
