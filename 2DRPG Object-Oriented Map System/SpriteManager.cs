using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Sprite Manager class is responsible for loading every sprite at once.
    /// </summary>
    public static class SpriteManager
{
        // Class to handle loading of all textures.
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Load Content method is responsible for loading all textures.
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            // Load all the textures at once.
            textures["north_wall"] = content.Load<Texture2D>("north_wall");
            textures["east_wall"] = content.Load<Texture2D>("east_wall");
            textures["west_wall"] = content.Load<Texture2D>("west_wall");
            textures["south_wall"] = content.Load<Texture2D>("south_wall");
            textures["top_west_wall"] = content.Load<Texture2D>("north_west_wall");
            textures["top_east_wall"] = content.Load<Texture2D>("north_east_wall");
            textures["bottom_west_wall"] = content.Load<Texture2D>("bottom_west_wall");
            textures["bottom_east_wall"] = content.Load<Texture2D>("bottom_east_wall");
            textures["ground_tile"] = content.Load<Texture2D>("ground_tile");
            textures["player"] = content.Load<Texture2D>("player");
            textures["exit_tile"] = content.Load<Texture2D>("exit_tile");
            textures["coin"] = content.Load<Texture2D>("coin");
            textures["spawn_tile"] = content.Load<Texture2D>("spawn_tile");
        }

        /// <summary>
        /// Get's a texture by name to be used, specifically for the Tilemap class.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Texture2D GetTexture(string name)
        {
            if (textures.ContainsKey(name))
            {
                return textures[name];
            }
            throw new System.Exception($"Texture {name} not found.");
        }
}
}
