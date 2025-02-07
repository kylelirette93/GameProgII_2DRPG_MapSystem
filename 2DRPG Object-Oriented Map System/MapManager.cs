using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class MapManager
{
        public Vector2 SpawnPoint { get; set; }
        private Vector2 _spawnPoint;
        private SpriteBatch _spriteBatch;
        List<string> levels = new List<string>();

        public MapManager()
        {

        }

        public Tilemap map { get; set; }

        public void LoadMaps()
        {
            for (int i = 0; i < 2; i++)
            {
                levels.Add(String.Format("Content/Level{0}.text", i));
            }
        }
        public void CreateMap()
        {
            map = new Tilemap();
            map.LoadFromFile(levels[1]);           
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
        }
}
}
