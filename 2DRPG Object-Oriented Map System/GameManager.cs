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

        // List of game objects.
        private List<GameObject> gameObjects = new List<GameObject>();
        private PlayerController player;
        GameObject playerObject;

        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();        
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all the textures at once.
            SpriteManager.LoadContent(Content);

            // Create the tilemap.
            Tilemap tilemap = new Tilemap();
            tilemap.GenerateProceduralMap(15, 10);

            // Create the player.
            playerObject = new GameObject();
            playerObject.AddComponent(new Transform());           
            playerObject.AddComponent(new Sprite(SpriteManager.GetTexture("player")));
            Sprite playerSprite = playerObject.GetComponent<Sprite>();
            playerSprite.Position = new Vector2(50, 50);
            playerObject.AddComponent(new Collider());
            AddGameObject(playerObject);

            GameObject tilemapObject = new GameObject();
            tilemapObject.AddComponent(tilemap);
            AddGameObject(tilemapObject);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach (var _gameObject in gameObjects)
            {
                var tilemap = _gameObject.GetComponent<Tilemap>();
                var playerSprite = playerObject.GetComponent<Sprite>();
                if (tilemap != null)
                {
                    tilemap.Draw(_spriteBatch);
                    playerSprite.Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }
    }
}
