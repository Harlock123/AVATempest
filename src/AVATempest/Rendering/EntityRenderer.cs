using System.Numerics;
using AVATempest.Entities;
using AVATempest.Entities.Enemies;
using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Rendering;

public class EntityRenderer
{
    private readonly VectorRenderer _vectorRenderer;

    public EntityRenderer(VectorRenderer vectorRenderer)
    {
        _vectorRenderer = vectorRenderer;
    }

    public void RenderPlayer(SKCanvas canvas, Player player, Tube tube, float time)
    {
        if (!player.IsAlive) return;

        var color = player.Color;

        // Flicker when invulnerable
        if (player.IsInvulnerable && (int)(time * 10) % 2 == 0)
        {
            color = color.WithAlpha(128);
        }

        var clawPoints = player.GetClawShape(tube);
        _vectorRenderer.DrawPolygon(canvas, clawPoints, color, 2f, false);
    }

    public void RenderProjectile(SKCanvas canvas, Projectile projectile, Tube tube)
    {
        if (!projectile.IsAlive) return;

        var pos = projectile.GetPosition(tube);
        var segment = tube.GetSegment(projectile.SegmentIndex);

        // Draw as a small line/dash in the direction of travel
        var direction = projectile.IsPlayerShot
            ? segment.GetInnerCenter() - segment.GetOuterCenter()
            : segment.GetOuterCenter() - segment.GetInnerCenter();
        direction = Vector2.Normalize(direction) * 10f;

        _vectorRenderer.DrawLine(canvas, pos - direction, pos + direction, projectile.Color, 2f);
    }

    public void RenderEnemy(SKCanvas canvas, Enemy enemy, Tube tube, float time)
    {
        if (!enemy.IsAlive) return;

        var pos = enemy.GetPosition(tube);

        switch (enemy)
        {
            case Flipper flipper:
                RenderFlipper(canvas, flipper, tube, pos, time);
                break;
            case Tanker tanker:
                RenderTanker(canvas, tanker, pos, time);
                break;
            case Spiker spiker:
                RenderSpiker(canvas, spiker, pos, time);
                break;
            case Fuseball fuseball:
                RenderFuseball(canvas, fuseball, pos, time);
                break;
            case Pulsar pulsar:
                RenderPulsar(canvas, pulsar, pos, time);
                break;
            default:
                // Generic enemy rendering
                _vectorRenderer.DrawCircle(canvas, pos, 10f, enemy.Color);
                break;
        }
    }

    private void RenderFlipper(SKCanvas canvas, Flipper flipper, Tube tube, Vector2 pos, float time)
    {
        float size = 12f;
        float angle = time * 3f + flipper.SegmentIndex;

        // Flipper is a rotating bow-tie shape
        var points = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            float a = angle + i * MathF.PI / 2;
            float r = (i % 2 == 0) ? size : size * 0.3f;
            points[i] = pos + new Vector2(MathF.Cos(a) * r, MathF.Sin(a) * r);
        }

        _vectorRenderer.DrawPolygon(canvas, points, flipper.Color, 2f);
    }

    private void RenderTanker(SKCanvas canvas, Tanker tanker, Vector2 pos, float time)
    {
        float size = 15f;
        float pulse = 1f + MathF.Sin(time * 2f) * 0.1f;

        // Tanker is a diamond shape
        var points = new Vector2[]
        {
            pos + new Vector2(0, -size * pulse),
            pos + new Vector2(size * pulse, 0),
            pos + new Vector2(0, size * pulse),
            pos + new Vector2(-size * pulse, 0)
        };

        _vectorRenderer.DrawPolygon(canvas, points, tanker.Color, 2f);

        // Inner diamond
        for (int i = 0; i < 4; i++)
            points[i] = Vector2.Lerp(pos, points[i], 0.5f);
        _vectorRenderer.DrawPolygon(canvas, points, tanker.Color, 1f, true, false);
    }

    private void RenderSpiker(SKCanvas canvas, Spiker spiker, Vector2 pos, float time)
    {
        float size = 10f;

        // Spiker is a spiral/helix shape
        var points = new List<Vector2>();
        for (int i = 0; i < 8; i++)
        {
            float a = time * 5f + i * MathF.PI / 4;
            float r = size * (0.5f + (i % 2) * 0.5f);
            points.Add(pos + new Vector2(MathF.Cos(a) * r, MathF.Sin(a) * r));
        }

        _vectorRenderer.DrawPolygon(canvas, points.ToArray(), spiker.Color, 1.5f);
    }

    private void RenderFuseball(SKCanvas canvas, Fuseball fuseball, Vector2 pos, float time)
    {
        float baseSize = 12f * fuseball.GetPulseScale();

        // Fuseball is a pulsing star
        var points = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float a = time * 8f + i * MathF.PI / 5;
            float r = (i % 2 == 0) ? baseSize : baseSize * 0.4f;
            points[i] = pos + new Vector2(MathF.Cos(a) * r, MathF.Sin(a) * r);
        }

        _vectorRenderer.DrawPolygon(canvas, points, fuseball.Color, 2f);
    }

    private void RenderPulsar(SKCanvas canvas, Pulsar pulsar, Vector2 pos, float time)
    {
        float size = 10f;
        var color = pulsar.Color;

        // Pulsar is a pulsing circle with rays
        _vectorRenderer.DrawCircle(canvas, pos, size, color, 2f);

        // Draw rays when electrifying
        if (pulsar.IsElectrifying)
        {
            for (int i = 0; i < 8; i++)
            {
                float a = i * MathF.PI / 4 + time * 10f;
                var rayEnd = pos + new Vector2(MathF.Cos(a), MathF.Sin(a)) * (size + 15f);
                _vectorRenderer.DrawLine(canvas, pos, rayEnd, color, 1f);
            }
        }
    }

    public void RenderSuperZapper(SKCanvas canvas, SuperZapper zapper, Vector2 center, float radius)
    {
        if (!zapper.IsActive) return;

        var color = zapper.Color;

        // Draw expanding circles
        for (int i = 0; i < 5; i++)
        {
            float r = radius * (1f - zapper.Intensity) * (0.2f + i * 0.2f);
            var ringColor = color.WithAlpha((byte)(255 * zapper.Intensity * (1f - i * 0.15f)));
            _vectorRenderer.DrawCircle(canvas, center, r, ringColor, 3f);
        }
    }
}
