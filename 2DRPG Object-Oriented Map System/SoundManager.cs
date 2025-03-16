using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Sound Manager class is responsible for playing sound effects and music.
    /// </summary>
    public static class SoundManager
{
        /// <summary>
        /// Play Sound method plays a sound effect from list of sound files.
        /// </summary>
        /// <param name="soundName"></param>
        /// <exception cref="Exception"></exception>
        public static void PlaySound(string soundName)
        {
            if (AssetManager.soundFiles.ContainsKey(soundName))
            {
                AssetManager.soundFiles[soundName].Play();
            }
            else
            {
                throw new Exception($"Sound {soundName} not found.");
            }
        }

        /// <summary>
        /// Play Music method plays a music file from list of sound files. It is looped.
        /// </summary>
        /// <param name="musicName"></param>
        /// <exception cref="Exception"></exception>
        public static void PlayMusic(string musicName)
        {
            if (AssetManager.soundFiles.ContainsKey(musicName))
            {
               SoundEffectInstance instance = AssetManager.soundFiles[musicName].CreateInstance();
               instance.IsLooped = true;
               instance.Play();
            }
            else
            {
                throw new Exception($"Music {musicName} not found.");
            }
        }
}
}
