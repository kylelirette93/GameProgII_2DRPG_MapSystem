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
            AnimationComponent playerAnimation = new AnimationComponent(AssetManager.GetTexture("player_idle"), 6, true);
            player.AddComponent(playerAnimation);
            player.AddComponent(new PlayerController("Player"));
            player.AddComponent(new Inventory());
            HealthComponent healthComponent = new HealthComponent(20);
            player.AddComponent(healthComponent);
            healthComponent.Initialize();
            player.AddComponent(new Weapon("fireball"));
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

        public static GameObject CreateEnemy(MapManager mapManager, string name, string textureName)
        {
            GameObject enemy = new GameObject(name);
            enemy.AddComponent(new Transform());
            enemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn(name);
            enemy.AddComponent(new Sprite(AssetManager.GetTexture(textureName)));
            enemy.AddComponent(new Collider(enemy.GetComponent<Sprite>().SpriteBounds));
            enemy.AddComponent(new MeleeEnemyAI(name));
            EnemyType type = new EnemyType();
            type.Type = "melee";
            enemy.AddComponent(type);
            HealthComponent healthComponent = new HealthComponent(5);
            enemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return enemy;
        }

        public static GameObject CreateRangedEnemy(MapManager mapManager, string name, string textureName)
        {
            GameObject rangedEnemy = new GameObject(name);
            rangedEnemy.AddComponent(new Transform());
            rangedEnemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn(name);
            rangedEnemy.AddComponent(new Sprite(AssetManager.GetTexture(textureName)));
            rangedEnemy.AddComponent(new Collider(rangedEnemy.GetComponent<Sprite>().SpriteBounds));
            rangedEnemy.AddComponent(new RangedEnemyAI(name));
            EnemyType type = new EnemyType();
            type.Type = "ranged";
            rangedEnemy.AddComponent(type);
            HealthComponent healthComponent = new HealthComponent(5);
            rangedEnemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return rangedEnemy;
        }

        public static GameObject CreateGhostEnemy(MapManager mapManager, string name, string textureName)
        {
            GameObject ghostEnemy = new GameObject(name);
            ghostEnemy.AddComponent(new Transform());
            ghostEnemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn(name);
            ghostEnemy.AddComponent(new Sprite(AssetManager.GetTexture(textureName)));
            ghostEnemy.AddComponent(new Collider(ghostEnemy.GetComponent<Sprite>().SpriteBounds));
            ghostEnemy.AddComponent(new GhostEnemyAI(name));
            EnemyType type = new EnemyType();
            type.Type = "ghost";
            ghostEnemy.AddComponent(type);
            HealthComponent healthComponent = new HealthComponent(1);
            ghostEnemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return ghostEnemy;
        }

        public static GameObject CreateBossEnemy(MapManager mapManager)
        {
            GameObject bossEnemy = new GameObject("Boss");
            bossEnemy.AddComponent(new Transform());
            bossEnemy.GetComponent<Transform>().Position = mapManager.FindEnemySpawn("Boss");
            bossEnemy.AddComponent(new Sprite(AssetManager.GetTexture("Boss")));
            bossEnemy.AddComponent(new Collider(bossEnemy.GetComponent<Sprite>().SpriteBounds));
            bossEnemy.AddComponent(new BossEnemyAI("Boss"));
            bossEnemy.AddComponent(new DisplayIcon(bossEnemy.GetComponent<Transform>().Position - new Vector2(100, 100)));
            EnemyType type = new EnemyType();
            type.Type = "boss";
            bossEnemy.AddComponent(type);
            HealthComponent healthComponent = new HealthComponent(15);
            bossEnemy.AddComponent(healthComponent);
            healthComponent.Initialize();
            return bossEnemy;
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

        public static GameObject CreateLightningStrike()
        {
            GameObject lightningStrike = new GameObject("lightningstrike");
            lightningStrike.AddComponent(new Transform());
            lightningStrike.AddComponent(new Sprite(AssetManager.GetTexture("groundstrike")));
            AnimationComponent lightningAnimation = new AnimationComponent(AssetManager.GetTexture("lightningstrike"), 6, false);
            lightningStrike.AddComponent(lightningAnimation);
            return lightningStrike;
        }

        public static GameObject CreateFireball(Vector2 position)
        {
            GameObject fireball = new GameObject("fireball");
            fireball.AddComponent(new Transform());
            fireball.GetComponent<Transform>().Position = position;
            fireball.AddComponent(new Sprite(AssetManager.GetTexture("fireball")));
            fireball.AddComponent(new Collider(fireball.GetComponent<Sprite>().SpriteBounds));
            fireball.AddComponent(new PlayerProjectileComponent());
            return fireball;
        }

        public static GameObject CreateTurnArrow()
        {
            GameObject turnArrow = new GameObject("turnArrow");
            turnArrow.AddComponent(new Transform());
            turnArrow.AddComponent(new Sprite(AssetManager.GetTexture("turn_arrow")));
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

        public static GameObject CreateRandomItem(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
                Random random = new Random();
                int randomItem = random.Next(0, 4);
                switch (randomItem)
                {
                    case 0:
                        return CreateHealingPotion(mapManager, playerPosition, minDistance);
                    case 1:
                        return CreateFireballScroll(mapManager, playerPosition, minDistance);
                    case 2:
                        return CreateLightningScroll(mapManager, playerPosition, minDistance);
                    case 3:
                        return CreateForceScroll(mapManager, playerPosition, minDistance);
                    default:
                        return CreateHealingPotion(mapManager, playerPosition, minDistance);
                }
        }

        public static GameObject CreateHealingPotion(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
            Vector2 healingPotionPosition = mapManager.CurrentMap.FindItemSpawn(playerPosition, minDistance);
            GameObject potion = new GameObject("item");
            potion.AddComponent(new Transform());
            potion.GetComponent<Transform>().Position = healingPotionPosition;
            potion.AddComponent(new Sprite(AssetManager.GetTexture("healing_potion")));
            potion.AddComponent(new Collider(potion.GetComponent<Sprite>().SpriteBounds));
            potion.AddComponent(new HealingComponent("Healing Potion", "Heals for 2 health."));

            return potion;
        }

        public static GameObject CreateFireballScroll(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
            Vector2 fireballScrollPosition = mapManager.CurrentMap.FindItemSpawn(playerPosition, minDistance);
            GameObject scrollOfFireball = new GameObject("item");
            scrollOfFireball.AddComponent(new Transform());
            scrollOfFireball.GetComponent<Transform>().Position = fireballScrollPosition;
            scrollOfFireball.AddComponent(new Sprite(AssetManager.GetTexture("scroll_of_fireball")));
            scrollOfFireball.AddComponent(new Collider(scrollOfFireball.GetComponent<Sprite>().SpriteBounds));
            scrollOfFireball.AddComponent(new FireballScroll("Scroll of Fireball", "Deals 3 damage."));
            return scrollOfFireball;
        }

        public static GameObject CreateLightningScroll(MapManager mapManager, Vector2 playerPosition, float minDistance)
        {
            Vector2 lightningScrollPosition = mapManager.CurrentMap.FindItemSpawn(playerPosition, minDistance);
            GameObject scrollOfLightning = new GameObject("item");
            scrollOfLightning.AddComponent(new Transform());
            scrollOfLightning.GetComponent<Transform>().Position = lightningScrollPosition;
            scrollOfLightning.AddComponent(new Sprite(AssetManager.GetTexture("scroll_of_lightning")));
            scrollOfLightning.AddComponent(new Collider(scrollOfLightning.GetComponent<Sprite>().SpriteBounds));
            scrollOfLightning.AddComponent(new LightningScroll("Scroll of Lightning", "Deals 5 damage."));
            return scrollOfLightning;
        }

        public static GameObject CreateForceScroll(MapManager mapManager, Vector2 playerPosition, float minDistance) 
        {
            Vector2 forceScrollPosition = mapManager.CurrentMap.FindItemSpawn(playerPosition, minDistance);
            GameObject scrollOfForce = new GameObject("item");
            scrollOfForce.AddComponent(new Transform());
            scrollOfForce.GetComponent<Transform>().Position = forceScrollPosition;
            scrollOfForce.AddComponent(new Sprite(AssetManager.GetTexture("scroll_of_force")));
            scrollOfForce.AddComponent(new Collider(scrollOfForce.GetComponent<Sprite>().SpriteBounds));
            scrollOfForce.AddComponent(new ForceScroll("Scroll of Force", "Deals 10 damage."));
            return scrollOfForce;
        }

        public static GameObject CreateHurricane()
        {
            GameObject hurricane = new GameObject("hurricane");
            hurricane.AddComponent(new Transform());
            hurricane.AddComponent(new Sprite(AssetManager.GetTexture("blank")));
            AnimationComponent hurricaneAnimation = new AnimationComponent(AssetManager.GetTexture("hurricane"), 6, false);
            hurricane.AddComponent(hurricaneAnimation);
            return hurricane;
        }
        public static GameObject InventorySystem()
        {
            GameObject inventorySystem = new GameObject("Inventory System");
            inventorySystem.AddComponent(new Transform());
            return inventorySystem;
        }
    }
}
