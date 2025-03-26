using Microsoft.Xna.Framework;
using System;
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
            player.AddComponent(new Sprite(AssetManager.GetTexture("player")));
            player.AddComponent(new Collider(player.GetComponent<Sprite>().SpriteBounds));
            player.AddComponent(new PlayerController("Player"));
            player.AddComponent(new Inventory());
            HealthComponent healthComponent = new HealthComponent(20);
            player.AddComponent(healthComponent);
            healthComponent.Initialize();         
            return player;
        }

        public static GameObject CreatePlayerTest(MapManager mapManager)
        {
            GameObject player = new GameObject("player");
            player.AddComponent(new Transform());
            player.AddComponent(new PlayerController("Player"));
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
            enemy.AddComponent(new Sprite(AssetManager.GetTexture("enemy")));
            enemy.AddComponent(new Collider(enemy.GetComponent<Sprite>().SpriteBounds));
            enemy.AddComponent(new MeleeEnemyAI(name));
            HealthComponent healthComponent = new HealthComponent(5);
            enemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return enemy;
        }

        public static GameObject CreateRangedEnemy(MapManager mapManager, string name)
        {
            GameObject rangedEnemy = new GameObject(name);
            rangedEnemy.AddComponent(new Transform());
            rangedEnemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn(name);
            rangedEnemy.AddComponent(new Sprite(AssetManager.GetTexture("ranged_enemy")));
            rangedEnemy.AddComponent(new Collider(rangedEnemy.GetComponent<Sprite>().SpriteBounds));
            rangedEnemy.AddComponent(new RangedEnemyAI(name));
            HealthComponent healthComponent = new HealthComponent(5);
            rangedEnemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return rangedEnemy;
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

        public static GameObject CreateProjectile(Vector2 position)
        {
            GameObject projectile = new GameObject("projectile");
            projectile.AddComponent(new Transform());
            projectile.GetComponent<Transform>().Position = position;
            projectile.AddComponent(new Sprite(AssetManager.GetTexture("projectile")));
            projectile.AddComponent(new Collider(projectile.GetComponent<Sprite>().SpriteBounds));
            projectile.AddComponent(new ProjectileComponent());
            return projectile;
        }

        public static GameObject CreateTurnArrow()
        {
            GameObject turnArrow = new GameObject("turnArrow");
            turnArrow.AddComponent(new Transform());
            turnArrow.AddComponent(new Sprite(AssetManager.GetTexture("turn_arrow")));
            AnimationComponent arrowAnimation = new(AssetManager.GetTexture("turn_arrow_point"), 10, true);
            turnArrow.AddComponent(arrowAnimation);
            arrowAnimation.PlayAnimation();
            return turnArrow;
        }

        public static GameObject CreateExit(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
            Vector2 exitPosition = mapManager.CurrentMap.FindExitSpawn(playerPosition, minDistance);
            GameObject exit = new GameObject("exit");
            exit.AddComponent(new Transform());
            exit.GetComponent<Transform>().Position = exitPosition;
            exit.AddComponent(new Sprite(AssetManager.GetTexture("exit_tile")));
            exit.AddComponent(new Collider(exit.GetComponent<Sprite>().SpriteBounds));
            return exit;
        }

        public static GameObject CreateHealingPotion()
        {
            GameObject potion = new GameObject("Healing Potion");
            potion.AddComponent(new Transform());
            potion.AddComponent(new Sprite(AssetManager.GetTexture("healing_potion")));
            potion.AddComponent(new Collider(potion.GetComponent<Sprite>().SpriteBounds));
            potion.AddComponent(new HealingComponent("Healing Potion", "Heals for 2 health."));
            return potion;
        }

        public static GameObject InventorySystem()
        {
            GameObject inventorySystem = new GameObject("Inventory System");
            inventorySystem.AddComponent(new Transform());
            return inventorySystem;
        }
    }
}
