using System.Numerics;

namespace AVATempest.Levels;

public class Tube
{
    public TubeSegment[] Segments { get; }
    public int SegmentCount => Segments.Length;
    public bool IsOpen { get; }
    public Vector2 Center { get; private set; }
    public float OuterRadius { get; private set; }
    public float InnerRadius { get; private set; }

    public Tube(int segmentCount, bool isOpen = false)
    {
        Segments = new TubeSegment[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            Segments[i] = new TubeSegment(i);
        }
        IsOpen = isOpen;
    }

    public void GenerateCircular(Vector2 center, float innerRadius, float outerRadius)
    {
        Center = center;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;

        int count = IsOpen ? SegmentCount + 1 : SegmentCount;
        float angleStep = (IsOpen ? MathF.PI : MathF.PI * 2) / count;

        for (int i = 0; i < SegmentCount; i++)
        {
            float angle1 = i * angleStep - MathF.PI / 2;
            float angle2 = (i + 1) * angleStep - MathF.PI / 2;

            Segments[i].InnerLeft = center + new Vector2(
                MathF.Cos(angle1) * innerRadius,
                MathF.Sin(angle1) * innerRadius);
            Segments[i].InnerRight = center + new Vector2(
                MathF.Cos(angle2) * innerRadius,
                MathF.Sin(angle2) * innerRadius);
            Segments[i].OuterLeft = center + new Vector2(
                MathF.Cos(angle1) * outerRadius,
                MathF.Sin(angle1) * outerRadius);
            Segments[i].OuterRight = center + new Vector2(
                MathF.Cos(angle2) * outerRadius,
                MathF.Sin(angle2) * outerRadius);
        }
    }

    public void GenerateFromPoints(Vector2 center, Vector2[] outerPoints, float innerScale)
    {
        Center = center;

        if (outerPoints.Length != SegmentCount + (IsOpen ? 1 : 0))
            throw new ArgumentException("Point count mismatch");

        OuterRadius = outerPoints.Length > 0 ? Vector2.Distance(center, outerPoints[0]) : 0;
        InnerRadius = OuterRadius * innerScale;

        for (int i = 0; i < SegmentCount; i++)
        {
            int nextI = (i + 1) % outerPoints.Length;
            if (IsOpen && i == SegmentCount - 1)
                nextI = i + 1;

            Segments[i].OuterLeft = outerPoints[i];
            Segments[i].OuterRight = outerPoints[nextI];
            Segments[i].InnerLeft = Vector2.Lerp(center, outerPoints[i], innerScale);
            Segments[i].InnerRight = Vector2.Lerp(center, outerPoints[nextI], innerScale);
        }
    }

    public int WrapSegmentIndex(int index)
    {
        if (IsOpen)
            return Math.Clamp(index, 0, SegmentCount - 1);
        return ((index % SegmentCount) + SegmentCount) % SegmentCount;
    }

    public TubeSegment GetSegment(int index)
    {
        return Segments[WrapSegmentIndex(index)];
    }

    public int GetAdjacentSegment(int current, int direction)
    {
        int next = current + direction;
        if (IsOpen)
        {
            return Math.Clamp(next, 0, SegmentCount - 1);
        }
        return WrapSegmentIndex(next);
    }

    public void ClearSpikes()
    {
        foreach (var segment in Segments)
        {
            segment.SpikeDepth = 0;
        }
    }

    public void ClearElectrification()
    {
        foreach (var segment in Segments)
        {
            segment.IsElectrified = false;
            segment.ElectrificationTime = 0;
        }
    }
}
