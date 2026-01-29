using System.Numerics;
using AVATempest.Levels;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.Entities;

public class Player : Entity
{
    public override SKColor Color => ColorPalette.Player;

    public float MoveSpeed { get; set; } = 8f;
    public float FireCooldown { get; private set; }
    public float FireRate { get; set; } = 0.1f;
    public int SuperZapperCharges { get; set; } = 2;
    public bool IsInvulnerable { get; set; }
    public float InvulnerabilityTime { get; set; }

    private float _moveAccumulator;
    private const float MoveThreshold = 1f;

    public Player()
    {
        Depth = 1f; // Player is always at the outer edge
        SegmentIndex = 0;
    }

    public override void Update(float deltaTime, Tube tube)
    {
        if (FireCooldown > 0)
            FireCooldown -= deltaTime;

        if (InvulnerabilityTime > 0)
        {
            InvulnerabilityTime -= deltaTime;
            if (InvulnerabilityTime <= 0)
                IsInvulnerable = false;
        }
    }

    public void Move(int direction, Tube tube, float deltaTime)
    {
        _moveAccumulator += direction * MoveSpeed * deltaTime;

        while (_moveAccumulator >= MoveThreshold)
        {
            SegmentIndex = tube.GetAdjacentSegment(SegmentIndex, 1);
            _moveAccumulator -= MoveThreshold;
        }

        while (_moveAccumulator <= -MoveThreshold)
        {
            SegmentIndex = tube.GetAdjacentSegment(SegmentIndex, -1);
            _moveAccumulator += MoveThreshold;
        }
    }

    public bool CanFire() => FireCooldown <= 0;

    public Projectile? Fire(Tube tube)
    {
        if (!CanFire()) return null;

        FireCooldown = FireRate;
        return new Projectile
        {
            SegmentIndex = SegmentIndex,
            Depth = 0.95f,
            IsPlayerShot = true
        };
    }

    public bool CanUseSuperZapper() => SuperZapperCharges > 0;

    public void UseSuperZapper()
    {
        if (SuperZapperCharges > 0)
            SuperZapperCharges--;
    }

    public void ResetForLevel()
    {
        SuperZapperCharges = 2;
        FireCooldown = 0;
    }

    public void MakeInvulnerable(float duration)
    {
        IsInvulnerable = true;
        InvulnerabilityTime = duration;
    }

    public Vector2[] GetClawShape(Tube tube)
    {
        var segment = tube.GetSegment(SegmentIndex);
        var center = segment.GetOuterCenter();
        var innerCenter = segment.GetCenterAtDepth(0.85f);

        // Claw shape: three prongs
        var left = segment.OuterLeft;
        var right = segment.OuterRight;

        var innerLeft = Vector2.Lerp(left, innerCenter, 0.3f);
        var innerRight = Vector2.Lerp(right, innerCenter, 0.3f);
        var tipLeft = Vector2.Lerp(segment.GetPointAtDepth(0.7f, true), innerCenter, 0.5f);
        var tipRight = Vector2.Lerp(segment.GetPointAtDepth(0.7f, false), innerCenter, 0.5f);

        return
        [
            left,
            innerLeft,
            tipLeft,
            innerCenter,
            tipRight,
            innerRight,
            right
        ];
    }
}
