using System.Numerics;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.UI;

public class HighScoreScreen
{
    private readonly HighScoreManager _highScoreManager;
    private string _playerName = "AAA";
    private int _cursorPosition = 0;
    private bool _isActive;
    private int _finalScore;
    private int _finalLevel;

    public bool IsActive => _isActive;
    public event Action? OnComplete;

    public HighScoreScreen(HighScoreManager highScoreManager)
    {
        _highScoreManager = highScoreManager;
    }

    public void Activate(int score, int level)
    {
        if (_highScoreManager.IsHighScore(score))
        {
            _isActive = true;
            _finalScore = score;
            _finalLevel = level;
            _playerName = "AAA";
            _cursorPosition = 0;
        }
    }

    public void HandleInput(char character)
    {
        if (!_isActive) return;

        if (char.IsLetter(character))
        {
            char[] chars = _playerName.ToCharArray();
            chars[_cursorPosition] = char.ToUpper(character);
            _playerName = new string(chars);
            _cursorPosition = Math.Min(_cursorPosition + 1, 2);
        }
    }

    public void MoveCursor(int direction)
    {
        if (!_isActive) return;
        _cursorPosition = Math.Clamp(_cursorPosition + direction, 0, 2);
    }

    public void CycleLetter(int direction)
    {
        if (!_isActive) return;

        char[] chars = _playerName.ToCharArray();
        char c = chars[_cursorPosition];
        c = (char)((c - 'A' + direction + 26) % 26 + 'A');
        chars[_cursorPosition] = c;
        _playerName = new string(chars);
    }

    public void Confirm()
    {
        if (!_isActive) return;

        _highScoreManager.AddScore(_playerName, _finalScore, _finalLevel);
        _isActive = false;
        OnComplete?.Invoke();
    }

    public void Render(SKCanvas canvas, float width, float height, VectorRenderer renderer, float time)
    {
        if (!_isActive) return;

        renderer.DrawTextWithGlow(canvas, "NEW HIGH SCORE!",
            new Vector2(width / 2, height / 3), ColorPalette.HighScoreText, 36f, centered: true);

        renderer.DrawTextWithGlow(canvas, $"{_finalScore:N0}",
            new Vector2(width / 2, height / 3 + 50), ColorPalette.ScoreText, 28f, centered: true);

        renderer.DrawText(canvas, "ENTER YOUR NAME:",
            new Vector2(width / 2, height / 2), ColorPalette.ScoreText, 20f, centered: true);

        // Draw name entry with cursor
        float letterSpacing = 40f;
        float startX = width / 2 - letterSpacing;

        for (int i = 0; i < 3; i++)
        {
            float x = startX + i * letterSpacing;
            float y = height / 2 + 50;

            var color = i == _cursorPosition ? ColorPalette.HighScoreText : ColorPalette.ScoreText;

            // Draw letter
            renderer.DrawTextWithGlow(canvas, _playerName[i].ToString(),
                new Vector2(x, y), color, 32f, centered: true);

            // Draw cursor indicator
            if (i == _cursorPosition)
            {
                float blink = (MathF.Sin(time * 6f) + 1f) / 2f;
                var cursorColor = color.WithAlpha((byte)(128 + 127 * blink));
                renderer.DrawLine(canvas, new Vector2(x - 15, y + 15), new Vector2(x + 15, y + 15), cursorColor, 2f);
            }
        }

        renderer.DrawText(canvas, "UP/DOWN - Change Letter    LEFT/RIGHT - Move    ENTER - Confirm",
            new Vector2(width / 2, height / 2 + 120), ColorPalette.ScoreText, 14f, centered: true);
    }
}
