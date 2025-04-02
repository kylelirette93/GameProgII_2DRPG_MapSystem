using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        bool isPausePressed = false;
        /// <summary>
        /// The current Game State.
        /// </summary>
        public GameState CurrentState { get; set; } = GameState.MainMenu;
        /// <summary>
        /// Main game constructor. Initializes the graphics device manager and sets the content root directory.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Made the game manager a singleton, to handle dynamic state changes.
        /// </summary>
        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Instance = this;
        }

        /// <summary>
        /// Enum for the different game states.
        /// </summary>
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        /// <summary>
        /// This method is responsible for changing the game state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(GameState state)
        {
            CurrentState = state;
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
            mapManager = new MapManager();
            currentScene = new Scene();
            currentScene.Initialize(mapManager);
            SoundManager.PlayMusic("mapMusic");
        }

        private void CleanupLevel()
        {
            if (currentScene != null && mapManager != null)
            {
                currentScene.Clear(mapManager);
                currentScene = null;
                mapManager.Clear();
                mapManager = null;
                TurnManager.Instance.TurnQueue.Clear();
                foreach (GameObject gameObject in ObjectManager.GameObjects)
                {
                    gameObject.Destroy();
                }
                ObjectManager.RemoveAll();
                SoundManager.StopMusic("mapMusic");
            }                      
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
            AssetManager.LoadContent(Content);
            uiManager = new UIManager();
        }
        /// <summary>
        /// Update method is responsible for updating the game state.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Q)) Exit();

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    uiManager.UpdateMainMenu(gameTime);
                    if (state.IsKeyDown(Keys.Enter))
                    {
                        CurrentState = GameState.Playing;
                        InitializeLevel();
                    }
                    break;
                case GameState.Playing:
                    ObjectManager.UpdateAll(gameTime);
                    currentScene.Update(gameTime);
                    if (state.IsKeyDown(Keys.P) && !isPausePressed)
                    {
                        CurrentState = GameState.Paused;
                        isPausePressed = true;
                    }
                    break;
                case GameState.Paused:
                    uiManager.UpdatePauseMenu(gameTime);
                    break;
                case GameState.GameOver:
                    uiManager.UpdateQuitMenu(gameTime);
                    if (state.IsKeyDown(Keys.M))
                    {
                        CleanupLevel();
                        CurrentState = GameState.MainMenu;
                    }
                    break;
            }
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

            switch (CurrentState)
            { 
                case GameState.MainMenu:
                    uiManager.DrawMainMenu(_spriteBatch);
                    break;
                case GameState.Playing:
                    ObjectManager.DrawAll(_spriteBatch);
                    break;
                case GameState.Paused:
                    uiManager.DrawPauseMenu(_spriteBatch);
                    break;
                case GameState.GameOver:
                    uiManager.DrawGameOverMenu(_spriteBatch);
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Play()
        {
            CurrentState = GameState.Playing;
            InitializeLevel();
        }

        public void MainMenu()
        {
            CurrentState = GameState.MainMenu;
        }

        public void Resume()         
        {
            CurrentState = GameState.Playing;
            isPausePressed = false;
        }
    }
}
