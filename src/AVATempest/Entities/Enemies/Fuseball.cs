using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public class Fuseball : Enemy
{
    public override SKColor Color => ColorPalette.Fuseball;

    private int _moveDirection;
    private float _bounceTimer;
    private float _pulsePhase;

    public Fuseball()
    {
        Points = 750;
        Speed = 0.3f;
        CanShoot = false;
        Depth = 1f; // Fuseballs are always at the top
        _moveDirection = Random.Shared.Next(2) * 2 - 1; // -1 or 1
    }

    public override void UpdateBehavior(float deltaTime, Tube tube, Player player)
    {
        base.Update(deltaTime, tube);

        _pulsePhase += deltaTime * 10f;
        _bounceTimer += deltaTime;

        // Move along the edge of the tube
        if (_bounceTimer >= 0.15f)
        {
            _bounceTimer = 0;

            int nextSegment = tube.GetAdjacentSegment(SegmentIndex, _moveDirection);

            // Check if we hit the edge of an open tube
            if (tube.IsOpen && nextSegment == SegmentIndex)
            {
                _moveDirection = -_moveDirection;
                nextSegment = tube.GetAdjacentSegment(SegmentIndex, _moveDirection);
            }

            SegmentIndex = nextSegment;
        }
    }

    public float GetPulseScale()
    {
        return 1f + MathF.Sin(_pulsePhase) * 0.2f;
    }
}
