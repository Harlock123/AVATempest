using System.Numerics;
using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Entities;

public abstract class Entity
{
    public int SegmentIndex { get; set; }
    public float Depth { get; set; }  // 0 = inner (far), 1 = outer (near/player edge)
    public bool IsAlive { get; set; } = true;
    public abstract SKColor Color { get; }

    public Vector2 GetPosition(Tube tube)
    {
        var segment = tube.GetSegment(SegmentIndex);
        return segment.GetCenterAtDepth(Depth);
    }

    public abstract void Update(float deltaTime, Tube tube);

    public virtual bool CollidesWith(Entity other, Tube tube, float threshold = 15f)
    {
        if (!IsAlive || !other.IsAlive) return false;
        if (SegmentIndex != other.SegmentIndex) return false;

        var pos = GetPosition(tube);
        var otherPos = other.GetPosition(tube);
        return Vector2.Distance(pos, otherPos) < threshold;
    }
}
