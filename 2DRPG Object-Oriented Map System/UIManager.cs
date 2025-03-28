﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// The UIManager class is responsible for rendering text to the screen.
    /// </summary>
    public class UIManager
{
        private SpriteFont turnDisplayfont;
        private Vector2 turnDisplayPosition = new Vector2(300, 350);
        private Vector2 dropShadowOffset = new Vector2(2, 2);
        
        public UIManager()
        {
            turnDisplayfont = AssetManager.GetFont("font");
        }

        /*public void Draw(SpriteBatch spriteBatch)
        {
            string participantName = TurnManager.Instance.CurrentTurnId;
            string displayText = "Player's Turn";

            if (participantName != null)
            {
                displayText = participantName + "'s Turn";
            }
            else
            {
                displayText = "Player's Turn";
            }
            spriteBatch.DrawString(turnDisplayfont, displayText, turnDisplayPosition + dropShadowOffset, Color.Black);
            spriteBatch.DrawString(turnDisplayfont, displayText, turnDisplayPosition, Color.White);
        }
        */
}
}
