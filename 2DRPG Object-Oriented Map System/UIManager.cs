using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SpriteFontPlus;
using System.IO;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The UIManager class is responsible for drawing the main menu, pause menu and game over menu.
    /// </summary>
    public class UIManager
    {
        private Vector2 displayPosition = new Vector2(0, 0);
        private Vector2 adventureButtonPosition = new Vector2(450, 290);
        private Vector2 returnButtonPosition = new Vector2(450, 290);
        private Vector2 resumeButtonPosition = new Vector2(450, 290);
        private Vector2 endlessButtonPosition = new Vector2(450, 350);
        private Vector2 quitButtonPosition = new Vector2(450, 410);
        Texture2D mainMenuBackground;
        Texture2D pauseMenuBackground;
        Texture2D gameOverMenuBackground;
        Texture2D buttonTexture;
        const string adventureText = "ADVENTURE";
        const string endlessText = "ENDLESS";
        const string returnText = "MAIN MENU";
        const string resumeText = "CONTINUE";
        const string quitText = "QUIT GAME";
        Vector2 buttonOffset = new Vector2(20, 12);
        UIButton adventureButton;
        UIButton endlessButton;
        UIButton returnButton;
        UIButton resumeButton;
        UIButton quitButton;
        List<UIButton> CurrentButtons = new List<UIButton>();

        Color defaultColor = Color.White;
        Color highlightedColor = Color.Gray;

        MouseState previousMouseState;

        public UIManager(GraphicsDevice graphicsDevice)
        {
            mainMenuBackground = AssetManager.GetTexture("menu");
            pauseMenuBackground = AssetManager.GetTexture("PauseMenu");
            gameOverMenuBackground = AssetManager.GetTexture("GameOverMenu");
            buttonTexture = AssetManager.GetTexture("button");
            string fontPath = Path.Combine("Content", "Minecraft.ttf"); // or whatever the filename is.
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(fontPath),
                25,
                1024,
                1024,
                new[]
                {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic
                }
            );

            SpriteFont buttonTextFont = fontBakeResult.CreateSpriteFont(graphicsDevice);
            adventureButton = new UIButton(buttonTexture, buttonTextFont, adventureText, adventureButtonPosition, buttonOffset);
            endlessButton = new UIButton(buttonTexture, buttonTextFont, endlessText, endlessButtonPosition, buttonOffset);
            returnButton = new UIButton(buttonTexture, buttonTextFont, returnText, returnButtonPosition, buttonOffset);
            resumeButton = new UIButton(buttonTexture, buttonTextFont, resumeText, resumeButtonPosition, buttonOffset);
            quitButton = new UIButton(buttonTexture, buttonTextFont, quitText, quitButtonPosition, buttonOffset);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mainMenuBackground, displayPosition, Color.White);
            adventureButton.Draw(spriteBatch);
            endlessButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawPauseMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pauseMenuBackground, displayPosition, Color.White);
            resumeButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawGameOverMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameOverMenuBackground, displayPosition, Color.White);
            returnButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            CurrentButtons.Clear();
            CurrentButtons.Add(adventureButton);
            CurrentButtons.Add(endlessButton);
            CurrentButtons.Add(quitButton);
            UpdateMouseState(CurrentButtons);
        }

        public void UpdatePauseMenu(GameTime gameTime)
        {
            CurrentButtons.Clear();
            CurrentButtons.Add(resumeButton);
            CurrentButtons.Add(quitButton);
            UpdateMouseState(CurrentButtons);
        }

        public void UpdateQuitMenu(GameTime gameTime)
        {
            CurrentButtons.Clear();
            CurrentButtons.Add(returnButton);
            CurrentButtons.Add(quitButton);
            UpdateMouseState(CurrentButtons);
        }

        private void UpdateMouseState(List<UIButton> buttons)
        {
            
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = mouseState.Position.ToVector2();

            foreach (var button in buttons)
            {
                button.Update(mousePosition);               

                if (button.IsWithinBounds(mousePosition) && !button.WasPressedThisFrame)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (button == adventureButton)
                        {
                            adventureButton.WasPressedThisFrame = true;
                            GameManager.Instance.AdventureClicked();
                        }
                        else if (button == endlessButton)
                        {
                            endlessButton.WasPressedThisFrame = true;
                            GameManager.Instance.EndlessClicked();
                        }
                        else if (button == returnButton)
                        {
                            returnButton.WasPressedThisFrame = true;
                            GameManager.Instance.MainMenu();
                        }
                        else if (button == resumeButton)
                        {
                            resumeButton.WasPressedThisFrame = true;
                            GameManager.Instance.Resume();
                        }
                        else if (button == quitButton)
                        {
                            quitButton.WasPressedThisFrame = true;
                            GameManager.Instance.Exit();
                        }
                    }
                }
                else if (mouseState.LeftButton == ButtonState.Released)
                {
                    button.WasPressedThisFrame = false;
                }
            }
            previousMouseState = mouseState;
        }
    }



    public class UIButton
    {
        private Texture2D texture;
        private SpriteFont font;
        private string text;
        private Vector2 position;
        private Vector2 textOffset;
        private Color defaultColor = Color.White;
        private Color highlightedColor = Color.Gray;
        private Color currentColor;
        public bool WasPressedThisFrame = false;


        public UIButton(Texture2D texture, SpriteFont font, string text, Vector2 position, Vector2 textOffset)
        {
            this.texture = texture;
            this.font = font;
            this.text = text;
            this.position = position;
            this.textOffset = textOffset;
            this.currentColor = defaultColor;
        }

        public bool IsWithinBounds(Vector2 mousePosition)
        {
            Rectangle buttonRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            return buttonRectangle.Contains(mousePosition);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, currentColor);
            spriteBatch.DrawString(font, text, position + textOffset, currentColor);
        }

        public void Update(Vector2 mousePosition)
        {
            if (IsWithinBounds(mousePosition))
            {
                currentColor = highlightedColor;
            }
            else
            {
                currentColor = defaultColor;
            }
        }
    }
}
