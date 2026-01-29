using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities;

public class Projectile : Entity
{
    public bool IsPlayerShot { get; set; }
    public float Speed { get; set; } = 3f;
    public override SKColor Color => IsPlayerShot ? ColorPalette.PlayerShot : ColorPalette.Flipper;

    public override void Update(float deltaTime, Tube tube)
    {
        if (IsPlayerShot)
        {
            // Player shots move toward center (decreasing depth)
            Depth -= Speed * deltaTime;
            if (Depth <= 0)
                IsAlive = false;
        }
        else
        {
            // Enemy shots move toward player (increasing depth)
            Depth += Speed * deltaTime;
            if (Depth >= 1)
                IsAlive = false;
        }
    }
}
