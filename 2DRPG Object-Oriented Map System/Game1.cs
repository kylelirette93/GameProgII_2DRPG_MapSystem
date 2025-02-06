using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Game1 : Game
    {
        // Entry point class.
        public static GraphicsDevice GameGraphicsDevice;
        private GraphicsDeviceManager _graphics;
        public GraphicsDeviceManager Graphics { get { return _graphics; } }

        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        GameObject playerObject;
        GameObject tilemapObject;
        public static Texture2D whitePixel;
        public int screenHeight;
        public int screenWidth;
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            GameGraphicsDevice = GraphicsDevice;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            screenHeight = _graphics.PreferredBackBufferHeight;
            screenWidth = _graphics.PreferredBackBufferWidth;
        }

        protected override void Initialize()
        {
            base.Initialize();        
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });

            // Load all the textures at once.
            SpriteManager.LoadContent(Content);

            GenerateTilemap();
            SpawnPlayer();
        }
        private void SpawnPlayer()
        {
            playerObject = GameObjectFactory.CreatePlayer();
            GameManager.AddGameObject(playerObject);
        }

        private void GenerateTilemap()
        {
            tilemapObject = GameObjectFactory.GenerateTilemap();
            GameManager.AddGameObject(tilemapObject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            GameManager.UpdateAll();
            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            GameManager.DrawAll(_spriteBatch);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
