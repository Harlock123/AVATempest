using System.Numerics;

namespace AVATempest.Levels;

public static class TubeShapes
{
    public static Tube CreateCircle(Vector2 center, float innerRadius, float outerRadius, int segments = 16)
    {
        var tube = new Tube(segments, false);
        tube.GenerateCircular(center, innerRadius, outerRadius);
        return tube;
    }

    public static Tube CreateSquare(Vector2 center, float innerRadius, float outerRadius)
    {
        var tube = new Tube(16, false);
        var points = new Vector2[16];

        // 4 segments per side
        float size = outerRadius;
        Vector2[] corners =
        [
            new(-size, -size),
            new(size, -size),
            new(size, size),
            new(-size, size)
        ];

        for (int side = 0; side < 4; side++)
        {
            Vector2 start = corners[side];
            Vector2 end = corners[(side + 1) % 4];
            for (int i = 0; i < 4; i++)
            {
                float t = i / 4f;
                points[side * 4 + i] = center + Vector2.Lerp(start, end, t);
            }
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreatePlus(Vector2 center, float innerRadius, float outerRadius)
    {
        // Plus shape has 12 corners
        var tube = new Tube(12, false);
        var points = new Vector2[12];

        float outer = outerRadius;
        float inner = outerRadius * 0.4f;

        // Plus shape points (clockwise from top-left of top arm)
        points[0] = center + new Vector2(-inner, -outer);  // Top arm, left
        points[1] = center + new Vector2(inner, -outer);   // Top arm, right
        points[2] = center + new Vector2(inner, -inner);   // Inner corner, top-right
        points[3] = center + new Vector2(outer, -inner);   // Right arm, top
        points[4] = center + new Vector2(outer, inner);    // Right arm, bottom
        points[5] = center + new Vector2(inner, inner);    // Inner corner, bottom-right
        points[6] = center + new Vector2(inner, outer);    // Bottom arm, right
        points[7] = center + new Vector2(-inner, outer);   // Bottom arm, left
        points[8] = center + new Vector2(-inner, inner);   // Inner corner, bottom-left
        points[9] = center + new Vector2(-outer, inner);   // Left arm, bottom
        points[10] = center + new Vector2(-outer, -inner); // Left arm, top
        points[11] = center + new Vector2(-inner, -inner); // Inner corner, top-left

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateTriangle(Vector2 center, float innerRadius, float outerRadius)
    {
        var tube = new Tube(12, false);
        var points = new Vector2[12];

        // 4 segments per side
        for (int side = 0; side < 3; side++)
        {
            float angle1 = side * MathF.PI * 2 / 3 - MathF.PI / 2;
            float angle2 = (side + 1) * MathF.PI * 2 / 3 - MathF.PI / 2;

            Vector2 p1 = center + new Vector2(MathF.Cos(angle1), MathF.Sin(angle1)) * outerRadius;
            Vector2 p2 = center + new Vector2(MathF.Cos(angle2), MathF.Sin(angle2)) * outerRadius;

            for (int i = 0; i < 4; i++)
            {
                float t = i / 4f;
                points[side * 4 + i] = Vector2.Lerp(p1, p2, t);
            }
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateStar(Vector2 center, float innerRadius, float outerRadius)
    {
        var tube = new Tube(10, false);
        var points = new Vector2[10];

        for (int i = 0; i < 10; i++)
        {
            float angle = i * MathF.PI * 2 / 10 - MathF.PI / 2;
            float radius = (i % 2 == 0) ? outerRadius : outerRadius * 0.5f;
            points[i] = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateFlatV(Vector2 center, float innerRadius, float outerRadius)
    {
        // Open tube (V-shape)
        var tube = new Tube(8, true);
        var points = new Vector2[9];

        float width = outerRadius * 2;
        float height = outerRadius;

        for (int i = 0; i <= 8; i++)
        {
            float t = i / 8f;
            float x = (t - 0.5f) * width;
            float y = -MathF.Abs(t - 0.5f) * height * 2 + height;
            points[i] = center + new Vector2(x, y);
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateClover(Vector2 center, float innerRadius, float outerRadius)
    {
        var tube = new Tube(16, false);
        var points = new Vector2[16];

        for (int i = 0; i < 16; i++)
        {
            float angle = i * MathF.PI * 2 / 16;
            // Clover formula: r = cos(2θ) modified
            float r = outerRadius * (0.6f + 0.4f * MathF.Abs(MathF.Cos(angle * 2)));
            points[i] = center + new Vector2(MathF.Cos(angle - MathF.PI / 2), MathF.Sin(angle - MathF.PI / 2)) * r;
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateHeart(Vector2 center, float innerRadius, float outerRadius)
    {
        var tube = new Tube(16, false);
        var points = new Vector2[16];

        for (int i = 0; i < 16; i++)
        {
            float t = i * MathF.PI * 2 / 16;
            // Heart parametric equations
            float x = 16 * MathF.Pow(MathF.Sin(t), 3);
            float y = 13 * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t);
            points[i] = center + new Vector2(x, -y) * (outerRadius / 20f);
        }

        tube.GenerateFromPoints(center, points, innerRadius / outerRadius);
        return tube;
    }

    public static Tube CreateLevel(int level, Vector2 center, float innerRadius, float outerRadius)
    {
        // Cycle through different shapes based on level
        return (level % 8) switch
        {
            0 => CreateCircle(center, innerRadius, outerRadius),
            1 => CreateSquare(center, innerRadius, outerRadius),
            2 => CreatePlus(center, innerRadius, outerRadius),
            3 => CreateTriangle(center, innerRadius, outerRadius),
            4 => CreateStar(center, innerRadius, outerRadius),
            5 => CreateFlatV(center, innerRadius, outerRadius),
            6 => CreateClover(center, innerRadius, outerRadius),
            7 => CreateHeart(center, innerRadius, outerRadius),
            _ => CreateCircle(center, innerRadius, outerRadius)
        };
    }
}
