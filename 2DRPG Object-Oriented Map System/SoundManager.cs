﻿using Microsoft.Xna.Framework.Audio;
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
        
        // Dictionary to store instances, so they can be stopped abruptly.
        private static Dictionary<string, SoundEffectInstance> playingMusic = new Dictionary<string, SoundEffectInstance>();

        /// <summary>
        /// This method retrives a sound effect from the asset manager and plays it.
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
        /// This method plays a music file from list of sound files. It is looped by default.
        /// </summary>
        /// <param name="musicName"></param>
        /// <exception cref="Exception"></exception>
        
        public static void PlayMusic(string musicName)
        {
            if (AssetManager.soundFiles.ContainsKey(musicName))
            {
                if (!playingMusic.ContainsKey(musicName))
                {
                    SoundEffectInstance instance = AssetManager.soundFiles[musicName].CreateInstance();
                    instance.IsLooped = true;
                    instance.Play();
                    playingMusic[musicName] = instance; 
                }
                else
                {
                    if (playingMusic[musicName].State != SoundState.Playing)
                    {
                        playingMusic[musicName].Play();
                    }
                }
            }
            else
            {
                throw new Exception($"Music {musicName} not found.");
            }
        }

        /// <summary>
        /// This method stops a music file from playing, if it is playing.
        /// </summary>
        /// <param name="musicName"></param>
        /// <exception cref="Exception"></exception>
        public static void StopMusic(string musicName)
        {
            if (playingMusic.ContainsKey(musicName))
            {
                if (playingMusic[musicName].State == SoundState.Playing)
                {
                    playingMusic[musicName].Stop(); // Stop the stored instance
                }
            }
            else
            {
                throw new Exception($"Music {musicName} not found or not playing.");
            }
        }
    }
}
