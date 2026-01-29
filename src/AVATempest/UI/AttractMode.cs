using System.Numerics;
using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.UI;

public class AttractMode
{
    private readonly HighScoreManager _highScoreManager;
    private float _demoTimer;
    private int _demoLevel;
    private float _transitionTimer;
    private AttractPhase _phase = AttractPhase.Title;

    private enum AttractPhase
    {
        Title,
        HighScores,
        Demo
    }

    public AttractMode(HighScoreManager highScoreManager)
    {
        _highScoreManager = highScoreManager;
    }

    public void Update(float deltaTime)
    {
        _demoTimer += deltaTime;
        _transitionTimer += deltaTime;

        // Cycle through phases every 10 seconds
        if (_transitionTimer > 10f)
        {
            _transitionTimer = 0;
            _phase = _phase switch
            {
                AttractPhase.Title => _highScoreManager.Entries.Count > 0 ? AttractPhase.HighScores : AttractPhase.Title,
                AttractPhase.HighScores => AttractPhase.Title,
                AttractPhase.Demo => AttractPhase.Title,
                _ => AttractPhase.Title
            };
        }

        // Change demo level periodically
        if (_demoTimer > 5f)
        {
            _demoTimer = 0;
            _demoLevel = (_demoLevel + 1) % 8;
        }
    }

    public void Render(SKCanvas canvas, float width, float height, VectorRenderer renderer, float time)
    {
        switch (_phase)
        {
            case AttractPhase.Title:
                RenderTitle(canvas, width, height, renderer, time);
                break;
            case AttractPhase.HighScores:
                RenderHighScores(canvas, width, height, renderer, time);
                break;
        }
    }

    private void RenderTitle(SKCanvas canvas, float width, float height, VectorRenderer renderer, float time)
    {
        var color = ColorPalette.GetLevelColor(_demoLevel);
        float pulse = 1f + MathF.Sin(time * 2f) * 0.1f;

        renderer.DrawTextWithGlow(canvas, "AVATEMPEST",
            new Vector2(width / 2, height / 3), color, 64f * pulse, centered: true);

        // Blinking start prompt
        float blink = (MathF.Sin(time * 4f) + 1f) / 2f;
        var promptColor = ColorPalette.ScoreText.WithAlpha((byte)(128 + 127 * blink));
        renderer.DrawTextWithGlow(canvas, "PRESS ENTER TO START",
            new Vector2(width / 2, height / 2 + 50), promptColor, 24f, centered: true);

        // Controls
        renderer.DrawText(canvas, "CONTROLS:",
            new Vector2(width / 2, height - 140), ColorPalette.ScoreText, 18f, centered: true);
        renderer.DrawText(canvas, "LEFT/RIGHT or A/D - Move    SPACE - Fire",
            new Vector2(width / 2, height - 110), ColorPalette.ScoreText, 16f, centered: true);
        renderer.DrawText(canvas, "TAB - Super Zapper    ESC - Pause",
            new Vector2(width / 2, height - 85), ColorPalette.ScoreText, 16f, centered: true);

        // High score
        if (_highScoreManager.TopScore > 0)
        {
            renderer.DrawTextWithGlow(canvas, $"HIGH SCORE: {_highScoreManager.TopScore:N0}",
                new Vector2(width / 2, height / 2 + 100), ColorPalette.HighScoreText, 20f, centered: true);
        }
    }

    private void RenderHighScores(SKCanvas canvas, float width, float height, VectorRenderer renderer, float time)
    {
        renderer.DrawTextWithGlow(canvas, "HIGH SCORES",
            new Vector2(width / 2, 80), ColorPalette.HighScoreText, 36f, centered: true);

        var entries = _highScoreManager.Entries;
        float startY = 150;
        float lineHeight = 35;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            float y = startY + i * lineHeight;

            var color = i == 0 ? ColorPalette.HighScoreText : ColorPalette.ScoreText;

            renderer.DrawText(canvas, $"{i + 1}.",
                new Vector2(width / 2 - 150, y), color, 20f);
            renderer.DrawText(canvas, entry.Name,
                new Vector2(width / 2 - 100, y), color, 20f);
            renderer.DrawText(canvas, $"{entry.Score:N0}",
                new Vector2(width / 2 + 50, y), color, 20f);
            renderer.DrawText(canvas, $"LV.{entry.Level + 1}",
                new Vector2(width / 2 + 150, y), color, 20f);
        }

        // Blinking prompt
        float blink = (MathF.Sin(time * 4f) + 1f) / 2f;
        var promptColor = ColorPalette.ScoreText.WithAlpha((byte)(128 + 127 * blink));
        renderer.DrawText(canvas, "PRESS ENTER TO START",
            new Vector2(width / 2, height - 60), promptColor, 20f, centered: true);
    }

    public void Reset()
    {
        _phase = AttractPhase.Title;
        _transitionTimer = 0;
        _demoTimer = 0;
    }
}
