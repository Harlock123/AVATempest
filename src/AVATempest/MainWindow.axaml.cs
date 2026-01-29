using System;
using System.Diagnostics;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AVATempest.Audio;
using AVATempest.Core;
using AVATempest.Rendering;

namespace AVATempest;

public partial class MainWindow : Window
{
    private readonly GameEngine _gameEngine;
    private readonly GameRenderer _gameRenderer;
    private readonly AudioManager _audioManager;
    private readonly Stopwatch _stopwatch;
    private readonly DispatcherTimer _gameTimer;
    private double _lastTime;

    public MainWindow()
    {
        InitializeComponent();

        _gameEngine = new GameEngine();
        _gameRenderer = new GameRenderer();
        _audioManager = new AudioManager();
        _stopwatch = Stopwatch.StartNew();

        // Initialize game canvas
        GameCanvas.Initialize(_gameEngine, _gameRenderer);

        // Set up input handling
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        // Handle window size changes
        PropertyChanged += (s, e) =>
        {
            if (e.Property == ClientSizeProperty)
            {
                UpdateGameDimensions();
            }
        };

        // Initialize game after window is loaded
        Loaded += (s, e) =>
        {
            UpdateGameDimensions();
            _gameEngine.SetParticleSystem(_gameRenderer.ParticleSystem);
            _gameEngine.SetAudioManager(_audioManager);
        };

        // Set up game loop using DispatcherTimer (~60 FPS)
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _gameTimer.Tick += OnGameTick;
        _gameTimer.Start();
    }

    private void UpdateGameDimensions()
    {
        var width = (float)ClientSize.Width;
        var height = (float)ClientSize.Height;

        if (width <= 0 || height <= 0) return;

        var center = new Vector2(width / 2, height / 2);
        var radius = Math.Min(width, height) * 0.4f;
        var innerRadius = radius * 0.15f;

        _gameEngine.Initialize(center, innerRadius, radius);
    }

    private void OnGameTick(object? sender, EventArgs e)
    {
        double currentTime = _stopwatch.Elapsed.TotalSeconds;
        float deltaTime = (float)(currentTime - _lastTime);
        _lastTime = currentTime;

        // Cap delta time to prevent huge jumps
        deltaTime = Math.Min(deltaTime, 0.1f);

        _gameEngine.Update(deltaTime);
        GameCanvas.InvalidateVisual();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _gameEngine.Input.OnKeyDown(e.Key);
        e.Handled = true;
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        _gameEngine.Input.OnKeyUp(e.Key);
        e.Handled = true;
    }

    protected override void OnClosed(EventArgs e)
    {
        _gameTimer.Stop();
        _gameRenderer.Dispose();
        _audioManager.Dispose();
        base.OnClosed(e);
    }
}
