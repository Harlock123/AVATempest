using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public class Tanker : Enemy
{
    public override SKColor Color => ColorPalette.Tanker;

    public Tanker()
    {
        Points = 250;
        Speed = 0.1f; // Slower than Flipper
        CanShoot = false;
    }

    public override void UpdateBehavior(float deltaTime, Tube tube, Player player)
    {
        base.Update(deltaTime, tube);

        // Simple movement - just climb up the tube
        if (!IsAtTop)
        {
            Depth += Speed * deltaTime;
            if (Depth > 1f) Depth = 1f;
        }
    }

    public override List<Enemy> OnDestroyed(Tube tube)
    {
        IsAlive = false;

        // Spawn two Flippers when destroyed
        var flippers = new List<Enemy>();

        var flipper1 = new Flipper
        {
            SegmentIndex = tube.GetAdjacentSegment(SegmentIndex, -1),
            Depth = Depth
        };

        var flipper2 = new Flipper
        {
            SegmentIndex = tube.GetAdjacentSegment(SegmentIndex, 1),
            Depth = Depth
        };

        flippers.Add(flipper1);
        flippers.Add(flipper2);

        return flippers;
    }
}
