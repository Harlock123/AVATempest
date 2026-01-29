using System.Numerics;

namespace AVATempest.Levels;

public class TubeSegment
{
    public int Index { get; }
    public Vector2 InnerLeft { get; set; }
    public Vector2 InnerRight { get; set; }
    public Vector2 OuterLeft { get; set; }
    public Vector2 OuterRight { get; set; }

    // Spike that may exist on this segment (depth 0-1, 0 = inner, 1 = outer)
    public float SpikeDepth { get; set; }
    public bool HasSpike => SpikeDepth > 0;

    // For pulsar electrification
    public bool IsElectrified { get; set; }
    public float ElectrificationTime { get; set; }

    public TubeSegment(int index)
    {
        Index = index;
    }

    public Vector2 GetPointAtDepth(float depth, bool leftSide)
    {
        depth = Math.Clamp(depth, 0f, 1f);
        if (leftSide)
            return Vector2.Lerp(InnerLeft, OuterLeft, depth);
        else
            return Vector2.Lerp(InnerRight, OuterRight, depth);
    }

    public Vector2 GetCenterAtDepth(float depth)
    {
        var left = GetPointAtDepth(depth, true);
        var right = GetPointAtDepth(depth, false);
        return (left + right) / 2;
    }

    public Vector2 GetOuterCenter()
    {
        return (OuterLeft + OuterRight) / 2;
    }

    public Vector2 GetInnerCenter()
    {
        return (InnerLeft + InnerRight) / 2;
    }

    public float GetWidthAtDepth(float depth)
    {
        var left = GetPointAtDepth(depth, true);
        var right = GetPointAtDepth(depth, false);
        return Vector2.Distance(left, right);
    }
}
