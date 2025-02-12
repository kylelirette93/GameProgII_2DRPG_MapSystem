using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// Main game class. Initializes the game and runs the game loop.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Texture2D whitePixel;
        MapManager mapManager;
        private Scene currentScene;
        /// <summary>
        /// Main game constructor. Initializes the graphics device manager and sets the content root directory.
        /// </summary>
        public Game1()
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
            base.Initialize();
            InitializeLevel();
        }

        private void InitializeLevel()
        {
            // Load maps, create new scene, initialize current scene.
            mapManager = new MapManager();
            currentScene = new Scene();
            currentScene.Initialize(mapManager);
        }
        /// <summary>
        /// Load Content method is responsible for loading all textures.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });

            // Load all the textures at once.
            SpriteManager.LoadContent(Content);
        }
        /// <summary>
        /// Update method is responsible for updating the game state.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            GameManager.UpdateAll();
            base.Update(gameTime);          
        }
        /// <summary>
        /// Draw method is responsible for drawing the game state.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            GameManager.DrawAll(_spriteBatch);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
