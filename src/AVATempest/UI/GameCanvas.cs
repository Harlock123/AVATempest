using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using AVATempest.Core;
using AVATempest.Rendering;
using SkiaSharp;

namespace AVATempest.UI;

public class GameCanvas : Control
{
    private GameEngine? _gameEngine;
    private GameRenderer? _gameRenderer;

    public void Initialize(GameEngine engine, GameRenderer renderer)
    {
        _gameEngine = engine;
        _gameRenderer = renderer;
    }

    public override void Render(DrawingContext context)
    {
        if (_gameEngine == null || _gameRenderer == null)
        {
            base.Render(context);
            return;
        }

        var bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.Custom(new GameDrawOperation(bounds, _gameEngine, _gameRenderer));
    }

    private class GameDrawOperation : ICustomDrawOperation
    {
        private readonly Rect _bounds;
        private readonly GameEngine _engine;
        private readonly GameRenderer _renderer;

        public GameDrawOperation(Rect bounds, GameEngine engine, GameRenderer renderer)
        {
            _bounds = bounds;
            _engine = engine;
            _renderer = renderer;
        }

        public Rect Bounds => _bounds;

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other)
        {
            return other is GameDrawOperation op && op._bounds == _bounds;
        }

        public bool HitTest(Point p) => _bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null) return;

            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;

            if (_engine.State == GameState.Attract)
            {
                var tube = _engine.LevelManager.CurrentLevel?.Tube;
                var color = _engine.LevelManager.CurrentLevel?.PrimaryColor ?? ColorPalette.LevelColors[0];
                if (tube != null)
                {
                    _renderer.RenderAttractMode(canvas, (float)_bounds.Width, (float)_bounds.Height,
                        tube, color, _engine.GameTime);
                }
            }
            else
            {
                _renderer.Render(canvas, (float)_bounds.Width, (float)_bounds.Height, _engine);
            }
        }
    }
}
