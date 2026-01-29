using System.Numerics;
using AVATempest.Core;
using AVATempest.Entities;
using AVATempest.Entities.Enemies;
using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Rendering;

public class GameRenderer : IDisposable
{
    private readonly VectorRenderer _vectorRenderer;
    private readonly TubeRenderer _tubeRenderer;
    private readonly EntityRenderer _entityRenderer;
    private readonly ParticleSystem _particleSystem;
    private readonly HUDRenderer _hudRenderer;

    public ParticleSystem ParticleSystem => _particleSystem;

    public GameRenderer()
    {
        _vectorRenderer = new VectorRenderer();
        _tubeRenderer = new TubeRenderer(_vectorRenderer);
        _entityRenderer = new EntityRenderer(_vectorRenderer);
        _particleSystem = new ParticleSystem(_vectorRenderer);
        _hudRenderer = new HUDRenderer(_vectorRenderer);
    }

    public void Render(SKCanvas canvas, float width, float height, GameEngine engine)
    {
        // Clear to black
        canvas.Clear(SKColors.Black);

        var level = engine.LevelManager.CurrentLevel;
        if (level == null) return;

        var tube = level.Tube;
        var color = level.PrimaryColor;

        // Draw depth lines for 3D effect
        _tubeRenderer.RenderDepthLines(canvas, tube, color);

        // Draw the tube
        _tubeRenderer.Render(canvas, tube, color, engine.Player.SegmentIndex);

        // Draw Super Zapper effect
        _entityRenderer.RenderSuperZapper(canvas, engine.SuperZapper, tube.Center, tube.OuterRadius);

        // Draw enemies
        foreach (var enemy in engine.Enemies)
        {
            _entityRenderer.RenderEnemy(canvas, enemy, tube, engine.GameTime);
        }

        // Draw projectiles
        foreach (var projectile in engine.Projectiles)
        {
            _entityRenderer.RenderProjectile(canvas, projectile, tube);
        }

        // Draw player
        _entityRenderer.RenderPlayer(canvas, engine.Player, tube, engine.GameTime);

        // Draw particles
        _particleSystem.Render(canvas);

        // Draw HUD
        _hudRenderer.Render(canvas, width, height,
            engine.Score, engine.HighScore, engine.Lives,
            engine.LevelManager.CurrentLevelNumber,
            engine.Player.SuperZapperCharges, color);

        // Draw state-specific overlays
        switch (engine.State)
        {
            case GameState.GameOver:
                _hudRenderer.RenderGameOver(canvas, width, height, engine.Score);
                break;
            case GameState.Paused:
                _hudRenderer.RenderPaused(canvas, width, height);
                break;
            case GameState.LevelComplete:
                _hudRenderer.RenderLevelComplete(canvas, width, height,
                    engine.LevelManager.CurrentLevelNumber, engine.LevelBonus, engine.StateProgress);
                break;
        }
    }

    public void RenderAttractMode(SKCanvas canvas, float width, float height, Tube tube, SKColor color, float time)
    {
        canvas.Clear(SKColors.Black);

        // Draw the tube
        _tubeRenderer.RenderDepthLines(canvas, tube, color);
        _tubeRenderer.Render(canvas, tube, color);

        // Draw title
        float pulse = 1f + MathF.Sin(time * 2f) * 0.1f;
        _vectorRenderer.DrawTextWithGlow(canvas, "AVATEMPEST",
            new Vector2(width / 2, height / 3), color, 64f * pulse, centered: true);

        // Draw instructions
        float blink = (MathF.Sin(time * 4f) + 1f) / 2f;
        var instructionColor = ColorPalette.ScoreText.WithAlpha((byte)(128 + 127 * blink));

        _vectorRenderer.DrawTextWithGlow(canvas, "PRESS ENTER TO START",
            new Vector2(width / 2, height / 2 + 50), instructionColor, 24f, centered: true);

        // Draw controls
        _vectorRenderer.DrawText(canvas, "CONTROLS:",
            new Vector2(width / 2, height - 120), ColorPalette.ScoreText, 18f, centered: true);
        _vectorRenderer.DrawText(canvas, "LEFT/RIGHT or A/D - Move",
            new Vector2(width / 2, height - 90), ColorPalette.ScoreText, 16f, centered: true);
        _vectorRenderer.DrawText(canvas, "SPACE - Fire  |  TAB - Super Zapper  |  ESC - Pause",
            new Vector2(width / 2, height - 65), ColorPalette.ScoreText, 16f, centered: true);

        // Draw particles
        _particleSystem.Render(canvas);
    }

    public void Dispose()
    {
        _vectorRenderer.Dispose();
    }
}
