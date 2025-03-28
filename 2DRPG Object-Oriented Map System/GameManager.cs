﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Main game class. Initializes the game and runs the game loop.
    /// </summary>
    public class GameManager : Game
    {
        public GraphicsDeviceManager Graphics { get => _graphics; }
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Texture2D whitePixel;
        MapManager mapManager;
        private Scene currentScene;
        UIManager uiManager;
        /// <summary>
        /// Main game constructor. Initializes the graphics device manager and sets the content root directory.
        /// </summary>
        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initialize method is responsible for initializing the game.
        /// </summary>
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1080;  // Set custom width
            _graphics.PreferredBackBufferHeight = 480;  // Set custom height
            _graphics.ApplyChanges();  // Apply the changes

            base.Initialize();
        }

        private void InitializeLevel()
        {
            // Load maps, create new scene, initialize current scene.
            mapManager = new MapManager();
            currentScene = new Scene();
            currentScene.Initialize(mapManager);
            uiManager = new UIManager();
            SoundManager.PlayMusic("mapMusic");
        }
        /// <summary>
        /// Load Content method is responsible for loading all textures.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });
            Gizmo.Initialize(GraphicsDevice, _spriteBatch);

            // Load all the textures at once.
            AssetManager.LoadContent(Content);
            InitializeLevel();
        }
        /// <summary>
        /// Update method is responsible for updating the game state.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            ObjectManager.UpdateAll(gameTime);
            currentScene.Update(gameTime);
            base.Update(gameTime);          
        }
        /// <summary>
        /// Draw method is responsible for drawing the game state.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);

            _spriteBatch.Begin();
            ObjectManager.DrawAll(_spriteBatch);
            //uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
