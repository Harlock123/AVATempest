using System.Numerics;

namespace AVATempest.Core;

public static class CollisionDetection
{
    public static bool LineSegmentIntersection(
        Vector2 p1, Vector2 p2,
        Vector2 p3, Vector2 p4,
        out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        float d = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);
        if (Math.Abs(d) < 0.0001f)
            return false;

        float t = ((p1.X - p3.X) * (p3.Y - p4.Y) - (p1.Y - p3.Y) * (p3.X - p4.X)) / d;
        float u = -((p1.X - p2.X) * (p1.Y - p3.Y) - (p1.Y - p2.Y) * (p1.X - p3.X)) / d;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = new Vector2(
                p1.X + t * (p2.X - p1.X),
                p1.Y + t * (p2.Y - p1.Y));
            return true;
        }

        return false;
    }

    public static bool PointNearLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, float threshold)
    {
        float lineLengthSq = Vector2.DistanceSquared(lineStart, lineEnd);
        if (lineLengthSq < 0.0001f)
            return Vector2.Distance(point, lineStart) <= threshold;

        float t = Math.Max(0, Math.Min(1,
            Vector2.Dot(point - lineStart, lineEnd - lineStart) / lineLengthSq));

        Vector2 projection = lineStart + t * (lineEnd - lineStart);
        return Vector2.Distance(point, projection) <= threshold;
    }

    public static bool CircleIntersectsLine(Vector2 center, float radius, Vector2 lineStart, Vector2 lineEnd)
    {
        return PointNearLine(center, lineStart, lineEnd, radius);
    }

    public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        float lineLengthSq = Vector2.DistanceSquared(lineStart, lineEnd);
        if (lineLengthSq < 0.0001f)
            return Vector2.Distance(point, lineStart);

        float t = Math.Max(0, Math.Min(1,
            Vector2.Dot(point - lineStart, lineEnd - lineStart) / lineLengthSq));

        Vector2 projection = lineStart + t * (lineEnd - lineStart);
        return Vector2.Distance(point, projection);
    }
}
