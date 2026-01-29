using SkiaSharp;

namespace AVATempest.Rendering;

public static class ColorPalette
{
    // Classic Tempest level color schemes
    public static readonly SKColor[] LevelColors =
    [
        new SKColor(0, 128, 255),   // Blue
        new SKColor(255, 0, 0),     // Red
        new SKColor(255, 255, 0),   // Yellow
        new SKColor(0, 255, 255),   // Cyan
        new SKColor(0, 255, 0),     // Green
        new SKColor(255, 0, 255),   // Magenta
        new SKColor(255, 128, 0),   // Orange
        new SKColor(128, 255, 128), // Light Green
        new SKColor(255, 128, 128), // Pink
        new SKColor(128, 128, 255), // Light Blue
        new SKColor(255, 255, 128), // Light Yellow
        new SKColor(128, 255, 255), // Light Cyan
        new SKColor(255, 128, 255), // Light Magenta
        new SKColor(255, 192, 128), // Peach
        new SKColor(192, 255, 192), // Pale Green
        new SKColor(192, 192, 255), // Pale Blue
    ];

    // Entity colors
    public static readonly SKColor Player = new(255, 255, 0);        // Yellow claw
    public static readonly SKColor PlayerShot = new(255, 255, 255);  // White
    public static readonly SKColor Flipper = new(255, 0, 0);         // Red
    public static readonly SKColor Tanker = new(0, 255, 0);          // Green
    public static readonly SKColor Spiker = new(0, 255, 255);        // Cyan
    public static readonly SKColor Spike = new(0, 128, 128);         // Dark Cyan
    public static readonly SKColor Fuseball = new(255, 0, 255);      // Magenta
    public static readonly SKColor Pulsar = new(255, 128, 0);        // Orange
    public static readonly SKColor PulsarActive = new(255, 255, 0);  // Yellow when electrified
    public static readonly SKColor SuperZapper = new(255, 255, 255); // White flash

    // UI colors
    public static readonly SKColor ScoreText = new(255, 255, 255);
    public static readonly SKColor HighScoreText = new(255, 255, 0);
    public static readonly SKColor LivesIndicator = new(255, 255, 0);

    public static SKColor GetLevelColor(int level)
    {
        return LevelColors[level % LevelColors.Length];
    }

    public static SKColor WithAlpha(this SKColor color, byte alpha)
    {
        return new SKColor(color.Red, color.Green, color.Blue, alpha);
    }

    public static SKColor Lerp(SKColor a, SKColor b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new SKColor(
            (byte)(a.Red + (b.Red - a.Red) * t),
            (byte)(a.Green + (b.Green - a.Green) * t),
            (byte)(a.Blue + (b.Blue - a.Blue) * t),
            (byte)(a.Alpha + (b.Alpha - a.Alpha) * t));
    }
}
