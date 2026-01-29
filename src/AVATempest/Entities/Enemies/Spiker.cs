using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public class Spiker : Enemy
{
    public override SKColor Color => ColorPalette.Spiker;

    private bool _isDescending;
    private float _maxSpikeDepth;

    public Spiker()
    {
        Points = 50;
        Speed = 0.25f;
        CanShoot = false;
        _isDescending = false;
    }

    public override void UpdateBehavior(float deltaTime, Tube tube, Player player)
    {
        base.Update(deltaTime, tube);

        var segment = tube.GetSegment(SegmentIndex);

        if (_isDescending)
        {
            // Moving back down, leaving spike trail
            Depth -= Speed * deltaTime;
            segment.SpikeDepth = Math.Max(segment.SpikeDepth, Depth);

            if (Depth <= 0)
            {
                IsAlive = false;
            }
        }
        else
        {
            // Moving up
            Depth += Speed * deltaTime;
            _maxSpikeDepth = Math.Max(_maxSpikeDepth, Depth);

            if (Depth >= 0.8f + Random.Shared.NextSingle() * 0.15f)
            {
                // Start descending
                _isDescending = true;
            }
        }
    }
}
