using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Net.Mime;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The UIManager class is responsible for drawing the main menu, pause menu and game over menu.
    /// </summary>
    public class UIManager
    {
        private Vector2 displayPosition = new Vector2(0, 0);
        private Vector2 playButtonPosition = new Vector2(450, 250);
        private Vector2 quitButtonPosition = new Vector2(450, 350);
        Texture2D mainMenuBackground;
        Texture2D pauseMenuBackground;
        Texture2D gameOverMenuBackground;
        Texture2D button;
        SpriteFont buttonText;
        const string playText = "Play Game";
        const string quitText = "Quit Game";
        Vector2 buttonOffset = new Vector2(20, 2);
        UIButton PlayButton;
        UIButton QuitButton;

        Color defaultColor = Color.White;
        Color highlightedColor = Color.Gray;

        public UIManager()
        {
            mainMenuBackground = AssetManager.GetTexture("MainMenu");
            pauseMenuBackground = AssetManager.GetTexture("PauseMenu");
            gameOverMenuBackground = AssetManager.GetTexture("GameOverMenu");            
            button = AssetManager.GetTexture("button");
            buttonText = AssetManager.GetFont("font");
            PlayButton = new UIButton(button, playButtonPosition);
            QuitButton = new UIButton(button, quitButtonPosition);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mainMenuBackground, displayPosition, Color.White);
            PlayButton.DrawButton(spriteBatch);
            spriteBatch.DrawString(buttonText, playText, playButtonPosition + buttonOffset, Color.White);
            spriteBatch.Draw(button, quitButtonPosition, Color.White);
            QuitButton.DrawButton(spriteBatch);
            spriteBatch.DrawString(buttonText, quitText, quitButtonPosition + buttonOffset, Color.White);
        }

        public void DrawPauseMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pauseMenuBackground, displayPosition, Color.White);
        }

        public void DrawGameOverMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameOverMenuBackground, displayPosition, Color.White);
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            MouseState mouseState = new MouseState();

            if (mouseState.Position.ToVector2() == playButtonPosition)
            {
                Debug.WriteLine("Play button highlighted");
            }
        }
    }

    public class UIButton
    {
        Texture2D buttonTexture;
        public string buttonText;
        Vector2 buttonPosition;
        Color currentColor;
        Color defaultColor = Color.White;
        Color highlightedColor = Color.Gray;

        public UIButton(Texture2D buttonTexture, Vector2 buttonPosition)
        {
            this.buttonTexture = buttonTexture;
            this.buttonPosition = buttonPosition;
        }
        public bool IsWithinBounds(Vector2 mousePosition)
        {
            // Check if the mouse position is within bounds.
            if (mousePosition.X > buttonPosition.X + buttonPosition.X / 2 &&
                mousePosition.X < buttonPosition.X - buttonPosition.X / 2 &&
                mousePosition.Y > buttonPosition.Y + buttonPosition.Y / 2 &&
                mousePosition.Y < buttonPosition.Y - buttonPosition.Y / 2)
            {
                return true;
            }
            return false;
        }

        public void DrawButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, buttonPosition, currentColor);
        }
    }
}