using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Cutscene 
{
        bool isStarted = false;
        int cutsceneDuration = 5000;
        private GraphicsDevice GraphicsDevice;
        public Cutscene(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
        public void Update(GameTime gameTime)
        {
            while (gameTime.ElapsedGameTime.TotalMilliseconds < cutsceneDuration)
            {
                // Update cutscene elements here
                isStarted = true;
            }
            isStarted = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            
            // Draw cutscene elements here
        }
}
}
