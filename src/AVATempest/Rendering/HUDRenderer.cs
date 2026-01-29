using System.Numerics;
using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Rendering;

public class HUDRenderer
{
    private readonly VectorRenderer _vectorRenderer;

    public HUDRenderer(VectorRenderer vectorRenderer)
    {
        _vectorRenderer = vectorRenderer;
    }

    public void Render(SKCanvas canvas, float width, float height,
        int score, int highScore, int lives, int level, int superZapperCharges, SKColor levelColor)
    {
        float padding = 20f;

        // Score (top left)
        _vectorRenderer.DrawTextWithGlow(canvas, $"SCORE: {score:N0}",
            new Vector2(padding, 30), ColorPalette.ScoreText, 20f);

        // High score (top center)
        _vectorRenderer.DrawTextWithGlow(canvas, $"HIGH: {highScore:N0}",
            new Vector2(width / 2, 30), ColorPalette.HighScoreText, 20f, centered: true);

        // Level (top right)
        _vectorRenderer.DrawTextWithGlow(canvas, $"LEVEL {level + 1}",
            new Vector2(width - padding - 100, 30), levelColor, 20f);

        // Lives (bottom left) - draw small claw shapes
        DrawLives(canvas, new Vector2(padding, height - 30), lives, levelColor);

        // Super Zapper charges (bottom right)
        DrawSuperZapperCharges(canvas, new Vector2(width - padding - 60, height - 30), superZapperCharges);
    }

    private void DrawLives(SKCanvas canvas, Vector2 position, int lives, SKColor color)
    {
        _vectorRenderer.DrawText(canvas, "LIVES:", position, ColorPalette.ScoreText, 16f);

        for (int i = 0; i < lives; i++)
        {
            var lifePos = position + new Vector2(70 + i * 25, -5);
            DrawMiniClaw(canvas, lifePos, color);
        }
    }

    private void DrawMiniClaw(SKCanvas canvas, Vector2 pos, SKColor color)
    {
        // Small claw icon
        var points = new Vector2[]
        {
            pos + new Vector2(-8, 5),
            pos + new Vector2(-4, 0),
            pos + new Vector2(-2, -5),
            pos + new Vector2(0, -8),
            pos + new Vector2(2, -5),
            pos + new Vector2(4, 0),
            pos + new Vector2(8, 5)
        };

        _vectorRenderer.DrawPolygon(canvas, points, color, 1.5f, false, false);
    }

    private void DrawSuperZapperCharges(SKCanvas canvas, Vector2 position, int charges)
    {
        _vectorRenderer.DrawText(canvas, "ZAPPER:", position, ColorPalette.ScoreText, 16f);

        for (int i = 0; i < charges; i++)
        {
            var zapPos = position + new Vector2(70 + i * 20, -5);
            _vectorRenderer.DrawCircle(canvas, zapPos, 6f, ColorPalette.SuperZapper, 2f, false);
        }
    }

    public void RenderGameOver(SKCanvas canvas, float width, float height, int score)
    {
        _vectorRenderer.DrawTextWithGlow(canvas, "GAME OVER",
            new Vector2(width / 2, height / 2 - 50), new SKColor(255, 0, 0), 48f, centered: true);

        _vectorRenderer.DrawTextWithGlow(canvas, $"FINAL SCORE: {score:N0}",
            new Vector2(width / 2, height / 2 + 10), ColorPalette.ScoreText, 28f, centered: true);

        _vectorRenderer.DrawTextWithGlow(canvas, "PRESS ENTER TO PLAY AGAIN",
            new Vector2(width / 2, height / 2 + 70), ColorPalette.ScoreText, 20f, centered: true);

        _vectorRenderer.DrawTextWithGlow(canvas, "PRESS ESC TO QUIT",
            new Vector2(width / 2, height / 2 + 105), new SKColor(255, 100, 100), 18f, centered: true);
    }

    public void RenderPaused(SKCanvas canvas, float width, float height)
    {
        _vectorRenderer.DrawTextWithGlow(canvas, "PAUSED",
            new Vector2(width / 2, height / 2 - 20), ColorPalette.ScoreText, 48f, centered: true);

        _vectorRenderer.DrawTextWithGlow(canvas, "PRESS ENTER TO RESUME",
            new Vector2(width / 2, height / 2 + 50), ColorPalette.ScoreText, 20f, centered: true);

        _vectorRenderer.DrawTextWithGlow(canvas, "PRESS ESC TO QUIT",
            new Vector2(width / 2, height / 2 + 85), new SKColor(255, 100, 100), 18f, centered: true);
    }

    public void RenderLevelComplete(SKCanvas canvas, float width, float height, int level, int bonus, float progress)
    {
        var alpha = (byte)(255 * Math.Min(1f, progress * 2));
        var color = ColorPalette.ScoreText.WithAlpha(alpha);

        _vectorRenderer.DrawTextWithGlow(canvas, $"LEVEL {level + 1} COMPLETE",
            new Vector2(width / 2, height / 2 - 20), color, 36f, centered: true);

        if (progress > 0.3f)
        {
            _vectorRenderer.DrawTextWithGlow(canvas, $"BONUS: {bonus:N0}",
                new Vector2(width / 2, height / 2 + 30), color, 24f, centered: true);
        }
    }
}
