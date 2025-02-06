using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace _2DRPG_Object_Oriented_Map_System
{
    public static class GameObjectFactory
    {
        public static GameObject CreatePlayer()
        {
            GameObject player = new GameObject("player");
            player.AddComponent(new Transform());
            player.AddComponent(new Sprite(SpriteManager.GetTexture("player")));
            player.AddComponent(new Collider(player.GetComponent<Sprite>().SpriteBounds));
            player.AddComponent(new PlayerController());
            return player;
        }

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
