using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SpriteFontPlus;
using System.IO;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// UI Manager is responsible for drawing and updating various menus with corresponding buttons and text.
    /// </summary>
    public class UIManager
    {
        private Vector2 displayPosition = new Vector2(0, 0);
        private Vector2 menuTextPosition = new Vector2(300, 25);
        private Vector2 pauseTextPosition = new Vector2(450, 25);
        private Vector2 gameOverTextPosition = new Vector2(385, 25);
        private Vector2 gameWinTextPosition = new Vector2(410, 25);
        private Vector2 instructionsTextPosition = new Vector2(810, 100);

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

        MouseState previousMouseState;

        public UIManager(GraphicsDevice graphicsDevice)
        {
            menuBackground = AssetManager.GetTexture("menu");
            buttonTexture = AssetManager.GetTexture("button");
            CreateFonts(graphicsDevice);
            CreateButtons();
        }

        private void CreateFonts(GraphicsDevice graphicsDevice)
        {
            // Using sprite font plus to create a font from TTF file.
            string fontPath = Path.Combine("Content", "Minecraft.ttf");
            byte[] fontData = File.ReadAllBytes(fontPath);

            buttonTextFont = TtfFontBaker.Bake(fontData, 25, 1024, 1024, CharacterRanges()).CreateSpriteFont(graphicsDevice);
            instructionsTextFont = TtfFontBaker.Bake(fontData, 17, 512, 512, CharacterRanges()).CreateSpriteFont(graphicsDevice);
            titleTextFont = TtfFontBaker.Bake(fontData, 50, 1024, 1024, CharacterRanges()).CreateSpriteFont(graphicsDevice);
        }

        private CharacterRange[] CharacterRanges()
        {
            // Define the character ranges to include in the font.
            return new[] { CharacterRange.BasicLatin, CharacterRange.Latin1Supplement, CharacterRange.LatinExtendedA, CharacterRange.Cyrillic };
        }

        private void CreateButtons()
        {
            adventureButton = new UIButton(buttonTexture, buttonTextFont, adventureText, new Vector2(450, 290), buttonOffset);
            endlessButton = new UIButton(buttonTexture, buttonTextFont, endlessText, new Vector2(450, 350), buttonOffset);
            returnButton = new UIButton(buttonTexture, buttonTextFont, returnText, new Vector2(450, 350), buttonOffset);
            resumeButton = new UIButton(buttonTexture, buttonTextFont, resumeText, new Vector2(450, 350), buttonOffset);
            quitButton = new UIButton(buttonTexture, buttonTextFont, quitText, new Vector2(450, 410), buttonOffset);
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
            DrawMenu(spriteBatch, pauseText, pauseTextPosition);
            resumeButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawGameplayPanel(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(instructionsTextFont, instructionsText, instructionsTextPosition, Color.White);
            string levelText = "Level: " + GameManager.Instance.Level.ToString();
            spriteBatch.DrawString(instructionsTextFont, levelText, instructionsTextPosition + new Vector2(0, 20), Color.White);
        }

        public void DrawGameOverMenu(SpriteBatch spriteBatch)
        {
            DrawMenu(spriteBatch, gameOverText, gameOverTextPosition);
            returnButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawWinMenu(SpriteBatch spriteBatch)
        {
            DrawMenu(spriteBatch, winText, gameWinTextPosition);
            returnButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        private void DrawMenu(SpriteBatch spriteBatch, string menuTitle, Vector2 titlePosition)
        {
            spriteBatch.Draw(menuBackground, displayPosition, Color.White);
            spriteBatch.DrawString(titleTextFont, menuTitle, titlePosition, Color.White);
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            UpdateMenuButtons(gameTime, new List<UIButton> { adventureButton, endlessButton, quitButton });
        }

        public void UpdatePauseMenu(GameTime gameTime)
        {
            UpdateMenuButtons(gameTime, new List<UIButton> { resumeButton, quitButton });
        }

        public void UpdateQuitMenu(GameTime gameTime)
        {
            UpdateMenuButtons(gameTime, new List<UIButton> { returnButton, quitButton });
        }

        public void UpdateWinMenu(GameTime gameTime)
        {
            UpdateMenuButtons(gameTime, new List<UIButton> { returnButton, quitButton });
        }

        private void UpdateMenuButtons(GameTime gameTime, List<UIButton> buttons)
        {
            CurrentButtons = buttons;
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
                        HandleButtonClick(button);
                    }
                }
                else if (mouseState.LeftButton == ButtonState.Released)
                {
                    button.WasPressedThisFrame = false;
                }
            }
            previousMouseState = mouseState;
        }

        private void HandleButtonClick(UIButton button)
        {
            if (button == adventureButton) GameManager.Instance.AdventureClicked();
            else if (button == endlessButton) GameManager.Instance.EndlessClicked();
            else if (button == returnButton) GameManager.Instance.MainMenu();
            else if (button == resumeButton) GameManager.Instance.Resume();
            else if (button == quitButton) GameManager.Instance.Exit();
            if (button == adventureButton || button == endlessButton || button == returnButton || button == resumeButton || button == quitButton)
            {
                button.WasPressedThisFrame = true;
            }
        }
    }
}


/// <summary>
/// UI Button class is responsible for creating and managing a button's state. Determining if it's pressed or not.
/// </summary>
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
