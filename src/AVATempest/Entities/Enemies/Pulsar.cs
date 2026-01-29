using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public class Pulsar : Enemy
{
    public override SKColor Color => IsElectrifying ? ColorPalette.PulsarActive : ColorPalette.Pulsar;

    public bool IsElectrifying { get; private set; }
    public float ElectrifyDuration { get; set; } = 0.5f;

    private float _electrifyTimer;
    private float _electrifyCooldown;
    private readonly float _electrifyInterval;

    public Pulsar()
    {
        Points = 200;
        Speed = 0.12f;
        CanShoot = false;
        _electrifyInterval = 2f + Random.Shared.NextSingle() * 2f;
        _electrifyCooldown = _electrifyInterval * Random.Shared.NextSingle();
    }

    public override void UpdateBehavior(float deltaTime, Tube tube, Player player)
    {
        base.Update(deltaTime, tube);

        var segment = tube.GetSegment(SegmentIndex);

        if (IsElectrifying)
        {
            _electrifyTimer += deltaTime;
            segment.IsElectrified = true;
            segment.ElectrificationTime = ElectrifyDuration - _electrifyTimer;

            if (_electrifyTimer >= ElectrifyDuration)
            {
                IsElectrifying = false;
                _electrifyTimer = 0;
                _electrifyCooldown = _electrifyInterval;
                segment.IsElectrified = false;
            }
        }
        else
        {
            // Move up the tube
            if (!IsAtTop)
            {
                Depth += Speed * deltaTime;
                if (Depth > 1f) Depth = 1f;
            }
            else
            {
                // At top, periodically electrify
                _electrifyCooldown -= deltaTime;
                if (_electrifyCooldown <= 0)
                {
                    IsElectrifying = true;
                    _electrifyTimer = 0;
                }
            }
        }
    }
}
