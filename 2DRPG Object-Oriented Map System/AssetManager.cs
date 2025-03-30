using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Asset Manager class is responsible for loading all assets at once.
    /// </summary>
    public static class AssetManager
{
        // Class to handle loading of all textures.
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SoundEffect> soundFiles = new Dictionary<string, SoundEffect>();
        private static SpriteFont font;

        /// <summary>
        /// Load Content method is responsible for loading all textures, sound files, fonts etc.
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            if (textures.Count == 0)
            {
                // Load all the textures, sound files and fonts at once.
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
                textures["enemy"] = content.Load<Texture2D>("enemy");
                textures["projectile"] = content.Load<Texture2D>("projectile");
                textures["ranged_enemy"] = content.Load<Texture2D>("ranged_enemy");
                textures["obstacle_tile"] = content.Load<Texture2D>("obstacle_tile");
                textures["enemy_hurt"] = content.Load<Texture2D>("enemy_hurt");
                textures["player_hurt"] = content.Load<Texture2D>("player_hurt");
                textures["turn_arrow"] = content.Load<Texture2D>("turn_arrow");
                textures["turn_arrow_point"] = content.Load<Texture2D>("turn_arrow_point");
                textures["default_slot"] = content.Load<Texture2D>("default_slot");
                textures["ghost_enemy"] = content.Load<Texture2D>("ghost_enemy");
                textures["healing_potion"] = content.Load<Texture2D>("healing_potion");
                textures["scroll_of_fireball"] = content.Load<Texture2D>("scroll_of_fireball");
                textures["scroll_of_lightning"] = content.Load<Texture2D>("scroll_of_lightning");
                textures["pixel"] = content.Load<Texture2D>("pixel");
                textures["fireball"] = content.Load<Texture2D>("fireball");

                font = content.Load<SpriteFont>("font");
                soundFiles["hurtSound"] = content.Load<SoundEffect>("hurtSound");
                soundFiles["mapMusic"] = content.Load<SoundEffect>("mapMusic");
            }
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

        /// <summary>
        /// Get's a font by name and returns it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SpriteFont GetFont(string name)
        {
            if (font != null)
            {
                return font;
            }
            return null;
        }

        /// <summary>
        ///  Get's a sound effect by name and returns it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static SoundEffect GetSound(string name)
        {
            if (soundFiles.ContainsKey(name))
            {
                return soundFiles[name];
            }
            throw new System.Exception($"Sound {name} not found");
        }
}
}
