using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Mime;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The UIManager class is responsible for rendering text to the screen.
    /// </summary>
    public class UIManager
    {
        private Vector2 displayPosition = new Vector2(0, 0);
        Texture2D mainMenuBackground;
        Texture2D pauseMenuBackground;
        Texture2D gameOverMenuBackground;

        public UIManager()
        {
            mainMenuBackground = AssetManager.GetTexture("MainMenu");
            pauseMenuBackground = AssetManager.GetTexture("PauseMenu");
            gameOverMenuBackground = AssetManager.GetTexture("GameOverMenu");
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mainMenuBackground, displayPosition, Color.White);
        }

        public void DrawPauseMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pauseMenuBackground, displayPosition, Color.White);
        }

        public void DrawGameOverMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameOverMenuBackground, displayPosition, Color.White);
        }

        public void UpdateMainMenu()
        {

        }

        public void UpdatePauseMenu()
        {
           
        }

        public void UpdateGameOverMenu()
        {

        }
    }
}