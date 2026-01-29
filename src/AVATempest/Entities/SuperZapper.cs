using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities;

public class SuperZapper
{
    public bool IsActive { get; private set; }
    public float Duration { get; private set; }
    public float MaxDuration { get; } = 0.5f;
    public float Intensity => Duration / MaxDuration;

    public SKColor Color => ColorPalette.SuperZapper.WithAlpha((byte)(255 * Intensity));

    public void Activate()
    {
        IsActive = true;
        Duration = MaxDuration;
    }

    public void Update(float deltaTime)
    {
        if (!IsActive) return;

        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsActive = false;
            Duration = 0;
        }
    }
}
