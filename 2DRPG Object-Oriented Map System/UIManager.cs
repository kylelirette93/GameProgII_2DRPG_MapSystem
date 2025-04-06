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
        private Vector2 returnButtonPosition = new Vector2(450, 350);
        private Vector2 resumeButtonPosition = new Vector2(450, 350);
        private Vector2 endlessButtonPosition = new Vector2(450, 350);
        private Vector2 quitButtonPosition = new Vector2(450, 410);
        private Vector2 menuTextPosition = new Vector2(300, 25);
        private Vector2 pauseTextPosition = new Vector2(450, 25);
        private Vector2 gameOverTextPosition = new Vector2(385, 25);
        private Vector2 gameWinTextPosition = new Vector2(410, 25);
        private Vector2 instructionsTextPosition = new Vector2(810,100);
        Texture2D menuBackground;
        Texture2D buttonTexture;
        const string menuText = "THE SHIFTING DEEP";
        const string pauseText = "PAUSED";
        const string gameOverText = "GAME OVER";
        const string winText = "VICTORY!";
        const string adventureText = "ADVENTURE";
        const string endlessText = "ENDLESS";
        const string returnText = "MAIN MENU";
        const string resumeText = "CONTINUE";
        const string quitText = "QUIT GAME";
        int currentLevel = 1;
        string instructionsText = "INSTRUCTIONS:\n\n" +
            "Use the WASD to move.\n" +
            "Move into enemy to attack.\n" +
            "Press number key to use item.\n" +
            "Press 'ESC' to pause the game.\n\n" +
            "Good luck!\n\n";
        SpriteFont buttonTextFont;
        SpriteFont titleTextFont;
        SpriteFont instructionsTextFont;
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
            menuBackground = AssetManager.GetTexture("menu");
            buttonTexture = AssetManager.GetTexture("button");
            CreateButtonTextFont(graphicsDevice);
            CreateTitleTextFont(graphicsDevice);
            CreateInstructionsTextFont(graphicsDevice);
            adventureButton = new UIButton(buttonTexture, buttonTextFont, adventureText, adventureButtonPosition, buttonOffset);
            endlessButton = new UIButton(buttonTexture, buttonTextFont, endlessText, endlessButtonPosition, buttonOffset);
            returnButton = new UIButton(buttonTexture, buttonTextFont, returnText, returnButtonPosition, buttonOffset);
            resumeButton = new UIButton(buttonTexture, buttonTextFont, resumeText, resumeButtonPosition, buttonOffset);
            quitButton = new UIButton(buttonTexture, buttonTextFont, quitText, quitButtonPosition, buttonOffset);
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBackground, displayPosition, Color.White);
            spriteBatch.DrawString(titleTextFont, menuText, menuTextPosition, Color.White);
            adventureButton.Draw(spriteBatch);
            endlessButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawPauseMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBackground, displayPosition, Color.White);
            spriteBatch.DrawString(titleTextFont, pauseText, pauseTextPosition, Color.White);
            resumeButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawGameplayPanel(SpriteBatch spriteBatch)
        {
            Vector2 textSize = titleTextFont.MeasureString(instructionsText);
            Vector2 centerPosition = new Vector2(GameManager.Instance.GraphicsDevice.Viewport.Width / 2, instructionsTextPosition.Y);
            Vector2 textPosition = centerPosition - new Vector2(textSize.X / 2, 0);
            spriteBatch.DrawString(instructionsTextFont, instructionsText, instructionsTextPosition, Color.White);
            currentLevel = GameManager.Instance.Level;
            string levelText = "Level: " + currentLevel.ToString();
            spriteBatch.DrawString(instructionsTextFont, levelText, instructionsTextPosition + new Vector2(0, 20), Color.White);
            
        }

        public void DrawGameOverMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBackground, displayPosition, Color.White);
            spriteBatch.DrawString(titleTextFont, gameOverText, gameOverTextPosition, Color.White);
            returnButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawWinMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBackground, displayPosition, Color.White);
            spriteBatch.DrawString(titleTextFont, winText, gameWinTextPosition, Color.White);
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

        public void UpdateWinMenu(GameTime gameTime)
        {
            CurrentButtons.Clear();
            CurrentButtons.Add(returnButton);
            CurrentButtons.Add(quitButton);
            UpdateMouseState(CurrentButtons);
        }

        private void CreateButtonTextFont(GraphicsDevice graphicsDevice)
        {
            string fontPath = Path.Combine("Content", "Minecraft.ttf"); 
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

            buttonTextFont = fontBakeResult.CreateSpriteFont(graphicsDevice);
        }

        private void CreateInstructionsTextFont(GraphicsDevice graphicsDevice)
        {
            string fontPath = Path.Combine("Content", "Minecraft.ttf");
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(fontPath),
                17,
                512,
                512,
                new[]
                {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic
                }
            );

            instructionsTextFont = fontBakeResult.CreateSpriteFont(graphicsDevice);
        }

        private void CreateTitleTextFont(GraphicsDevice graphicsDevice)
        {
            string fontPath = Path.Combine("Content", "Minecraft.ttf");
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(fontPath),
                50,
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

            titleTextFont = fontBakeResult.CreateSpriteFont(graphicsDevice);
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
