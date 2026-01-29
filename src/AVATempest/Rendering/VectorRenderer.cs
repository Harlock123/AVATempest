using SkiaSharp;
using System.Numerics;

namespace AVATempest.Rendering;

public class VectorRenderer
{
    private readonly SKPaint _linePaint;
    private readonly SKPaint _glowPaint;
    private readonly SKPaint _textPaint;

    public VectorRenderer()
    {
        _linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        _glowPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        _textPaint = new SKPaint
        {
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Bold),
            TextSize = 24
        };
    }

    public void DrawLine(SKCanvas canvas, Vector2 start, Vector2 end, SKColor color, float width = 2f, bool glow = true)
    {
        if (glow)
        {
            DrawGlowLine(canvas, start, end, color, width);
        }
        else
        {
            _linePaint.Color = color;
            _linePaint.StrokeWidth = width;
            canvas.DrawLine(start.X, start.Y, end.X, end.Y, _linePaint);
        }
    }

    public void DrawGlowLine(SKCanvas canvas, Vector2 start, Vector2 end, SKColor color, float width = 2f)
    {
        // Outer glow layers
        for (int i = 3; i >= 1; i--)
        {
            _glowPaint.Color = color.WithAlpha((byte)(40 / i));
            _glowPaint.StrokeWidth = width + i * 4;
            canvas.DrawLine(start.X, start.Y, end.X, end.Y, _glowPaint);
        }

        // Core line
        _linePaint.Color = color;
        _linePaint.StrokeWidth = width;
        canvas.DrawLine(start.X, start.Y, end.X, end.Y, _linePaint);

        // Bright center
        _linePaint.Color = new SKColor(
            (byte)Math.Min(255, color.Red + 100),
            (byte)Math.Min(255, color.Green + 100),
            (byte)Math.Min(255, color.Blue + 100));
        _linePaint.StrokeWidth = width * 0.5f;
        canvas.DrawLine(start.X, start.Y, end.X, end.Y, _linePaint);
    }

    public void DrawPolygon(SKCanvas canvas, Vector2[] points, SKColor color, float width = 2f, bool closed = true, bool glow = true)
    {
        if (points.Length < 2) return;

        for (int i = 0; i < points.Length - 1; i++)
        {
            DrawLine(canvas, points[i], points[i + 1], color, width, glow);
        }

        if (closed && points.Length > 2)
        {
            DrawLine(canvas, points[^1], points[0], color, width, glow);
        }
    }

    public void DrawCircle(SKCanvas canvas, Vector2 center, float radius, SKColor color, float width = 2f, bool glow = true)
    {
        if (glow)
        {
            // Outer glow layers
            for (int i = 3; i >= 1; i--)
            {
                _glowPaint.Color = color.WithAlpha((byte)(40 / i));
                _glowPaint.StrokeWidth = width + i * 4;
                canvas.DrawCircle(center.X, center.Y, radius, _glowPaint);
            }
        }

        _linePaint.Color = color;
        _linePaint.StrokeWidth = width;
        canvas.DrawCircle(center.X, center.Y, radius, _linePaint);
    }

    public void DrawText(SKCanvas canvas, string text, Vector2 position, SKColor color, float size = 24f, bool centered = false)
    {
        _textPaint.Color = color;
        _textPaint.TextSize = size;

        if (centered)
        {
            float textWidth = _textPaint.MeasureText(text);
            position.X -= textWidth / 2;
        }

        canvas.DrawText(text, position.X, position.Y, _textPaint);
    }

    public void DrawTextWithGlow(SKCanvas canvas, string text, Vector2 position, SKColor color, float size = 24f, bool centered = false)
    {
        _textPaint.TextSize = size;

        float textWidth = 0;
        if (centered)
        {
            textWidth = _textPaint.MeasureText(text);
            position.X -= textWidth / 2;
        }

        // Glow effect
        for (int i = 3; i >= 1; i--)
        {
            _textPaint.Color = color.WithAlpha((byte)(60 / i));
            _textPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, i * 2);
            canvas.DrawText(text, position.X, position.Y, _textPaint);
        }

        _textPaint.MaskFilter = null;
        _textPaint.Color = color;
        canvas.DrawText(text, position.X, position.Y, _textPaint);
    }

    public void Dispose()
    {
        _linePaint.Dispose();
        _glowPaint.Dispose();
        _textPaint.Dispose();
    }
}
