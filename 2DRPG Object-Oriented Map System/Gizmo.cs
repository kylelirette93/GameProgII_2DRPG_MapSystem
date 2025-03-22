using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace _2DRPG_Object_Oriented_Map_System
{

public static class Gizmo
{
    private static Texture2D _pixel;
    private static SpriteBatch spriteBatch;

    public static void Initialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        Gizmo.spriteBatch = spriteBatch;
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);

        spriteBatch.Draw(_pixel,
            new Rectangle(
                (int)start.X,
                (int)start.Y,
                (int)edge.Length(),
                1),
            null,
            color,
            angle,
            new Vector2(0, 0),
            SpriteEffects.None,
            0);
    }

    public static void DrawRectangle(Rectangle rectangle, Color color)
    {
        DrawLine(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), color);
        DrawLine(new Vector2(rectangle.Right, rectangle.Top), new Vector2(rectangle.Right, rectangle.Bottom), color);
        DrawLine(new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Bottom), color);
        DrawLine(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Top), color);
    }

    public static void DrawCircle(Vector2 center, float radius, Color color, int segments = 36)
    {
        Vector2 previousPoint = new Vector2(center.X + radius, center.Y);
        for (int i = 1; i <= segments; i++)
        {
            float angle = MathHelper.TwoPi * (float)i / segments;
            Vector2 currentPoint = new Vector2(center.X + radius * (float)Math.Cos(angle), center.Y + radius * (float)Math.Sin(angle));
            DrawLine(previousPoint, currentPoint, color);
            previousPoint = currentPoint;
        }
    }
}
}
