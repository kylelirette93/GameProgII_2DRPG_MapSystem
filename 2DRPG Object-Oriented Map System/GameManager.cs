using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class GameManager : Game
    {
        private GraphicsDeviceManager _graphics;
        public GraphicsDeviceManager Graphics { get { return _graphics; } }

        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        private List<GameEntity> _gameEntites = new List<GameEntity>();

        Tilemap tilemap;
        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();      
            // Initialize tilemap.
            tilemap = new Tilemap(10, 15);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            tilemap.Draw(_spriteBatch);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
