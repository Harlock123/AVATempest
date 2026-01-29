using System.Numerics;
using SkiaSharp;

namespace AVATempest.Rendering;

public class Particle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public SKColor Color { get; set; }
    public float Life { get; set; }
    public float MaxLife { get; set; }
    public float Size { get; set; }

    public float LifeRatio => Life / MaxLife;
    public bool IsAlive => Life > 0;

    public void Update(float deltaTime)
    {
        Position += Velocity * deltaTime;
        Velocity *= 0.98f; // Drag
        Life -= deltaTime;
    }
}

public class ParticleSystem
{
    private readonly List<Particle> _particles = new();
    private readonly VectorRenderer _vectorRenderer;
    private const int MaxParticles = 500;

    public ParticleSystem(VectorRenderer vectorRenderer)
    {
        _vectorRenderer = vectorRenderer;
    }

    public void SpawnExplosion(Vector2 position, SKColor color, int count = 20)
    {
        for (int i = 0; i < count && _particles.Count < MaxParticles; i++)
        {
            float angle = Random.Shared.NextSingle() * MathF.PI * 2;
            float speed = 100f + Random.Shared.NextSingle() * 200f;

            _particles.Add(new Particle
            {
                Position = position,
                Velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed,
                Color = color,
                Life = 0.5f + Random.Shared.NextSingle() * 0.5f,
                MaxLife = 1f,
                Size = 2f + Random.Shared.NextSingle() * 2f
            });
        }
    }

    public void SpawnLine(Vector2 start, Vector2 end, SKColor color, int count = 10)
    {
        for (int i = 0; i < count && _particles.Count < MaxParticles; i++)
        {
            float t = Random.Shared.NextSingle();
            var pos = Vector2.Lerp(start, end, t);
            float angle = Random.Shared.NextSingle() * MathF.PI * 2;
            float speed = 50f + Random.Shared.NextSingle() * 100f;

            _particles.Add(new Particle
            {
                Position = pos,
                Velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed,
                Color = color,
                Life = 0.3f + Random.Shared.NextSingle() * 0.3f,
                MaxLife = 0.6f,
                Size = 1f + Random.Shared.NextSingle() * 2f
            });
        }
    }

    public void SpawnWarpEffect(Vector2 center, float radius, SKColor color)
    {
        for (int i = 0; i < 50 && _particles.Count < MaxParticles; i++)
        {
            float angle = Random.Shared.NextSingle() * MathF.PI * 2;
            float r = radius * (0.5f + Random.Shared.NextSingle() * 0.5f);
            var pos = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * r;

            // Particles move toward center
            var velocity = (center - pos) * 2f;

            _particles.Add(new Particle
            {
                Position = pos,
                Velocity = velocity,
                Color = color,
                Life = 0.5f + Random.Shared.NextSingle() * 0.5f,
                MaxLife = 1f,
                Size = 3f
            });
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            _particles[i].Update(deltaTime);
            if (!_particles[i].IsAlive)
            {
                _particles.RemoveAt(i);
            }
        }
    }

    public void Render(SKCanvas canvas)
    {
        foreach (var particle in _particles)
        {
            var color = particle.Color.WithAlpha((byte)(255 * particle.LifeRatio));
            var size = particle.Size * particle.LifeRatio;

            // Draw particle as a small line in direction of movement
            var dir = Vector2.Normalize(particle.Velocity) * size * 2;
            if (float.IsNaN(dir.X)) dir = new Vector2(size, 0);

            _vectorRenderer.DrawLine(canvas,
                particle.Position - dir,
                particle.Position + dir,
                color, size, glow: false);
        }
    }

    public void Clear()
    {
        _particles.Clear();
    }
}
