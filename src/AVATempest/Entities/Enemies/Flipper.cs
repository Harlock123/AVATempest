using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public class Flipper : Enemy
{
    public override SKColor Color => ColorPalette.Flipper;

    private float _flipTimer;
    private float _flipInterval;
    private bool _isFlipping;
    private int _flipDirection;
    private float _flipProgress;

    public Flipper()
    {
        Points = 150;
        Speed = 0.2f;
        CanShoot = false;
        _flipInterval = 0.5f + Random.Shared.NextSingle() * 1.5f;
    }

    public override void UpdateBehavior(float deltaTime, Tube tube, Player player)
    {
        base.Update(deltaTime, tube);

        if (_isFlipping)
        {
            // Animate the flip between segments
            _flipProgress += deltaTime * 4f;
            if (_flipProgress >= 1f)
            {
                SegmentIndex = tube.GetAdjacentSegment(SegmentIndex, _flipDirection);
                _isFlipping = false;
                _flipProgress = 0;
                _flipTimer = 0;
            }
        }
        else if (IsAtTop)
        {
            // At top, try to flip toward player
            _flipTimer += deltaTime;
            if (_flipTimer >= _flipInterval)
            {
                int targetDirection = 0;
                if (player.SegmentIndex != SegmentIndex)
                {
                    // Determine shortest path to player
                    int diff = player.SegmentIndex - SegmentIndex;
                    if (!tube.IsOpen)
                    {
                        // Handle wrapping
                        if (Math.Abs(diff) > tube.SegmentCount / 2)
                            diff = diff > 0 ? diff - tube.SegmentCount : diff + tube.SegmentCount;
                    }
                    targetDirection = Math.Sign(diff);
                }

                if (targetDirection != 0)
                {
                    _isFlipping = true;
                    _flipDirection = targetDirection;
                    _flipInterval = 0.3f + Random.Shared.NextSingle() * 0.5f;
                }
                else
                {
                    _flipTimer = 0;
                }
            }
        }
        else
        {
            // Moving up the tube
            Depth += Speed * deltaTime;
            if (Depth > 1f) Depth = 1f;
        }
    }

    public float GetFlipOffset()
    {
        if (!_isFlipping) return 0;
        // Smooth flip animation
        return MathF.Sin(_flipProgress * MathF.PI) * 0.5f * _flipDirection;
    }
}
