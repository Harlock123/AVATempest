using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Entities.Enemies;

public abstract class Enemy : Entity
{
    public int Points { get; protected set; }
    public float Speed { get; set; } = 0.15f;
    public bool IsAtTop => Depth >= 0.95f;
    public bool CanShoot { get; protected set; }
    public float ShootCooldown { get; set; }
    public float ShootRate { get; set; } = 2f;

    public abstract void UpdateBehavior(float deltaTime, Tube tube, Player player);

    public override void Update(float deltaTime, Tube tube)
    {
        if (ShootCooldown > 0)
            ShootCooldown -= deltaTime;
    }

    public virtual Projectile? TryShoot()
    {
        if (!CanShoot || ShootCooldown > 0) return null;

        ShootCooldown = ShootRate;
        return new Projectile
        {
            SegmentIndex = SegmentIndex,
            Depth = Depth + 0.05f,
            IsPlayerShot = false,
            Speed = 1.5f
        };
    }

    public virtual List<Enemy> OnDestroyed(Tube tube)
    {
        IsAlive = false;
        return [];
    }
}
