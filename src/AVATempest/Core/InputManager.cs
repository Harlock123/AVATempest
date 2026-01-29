using Avalonia.Input;

namespace AVATempest.Core;

public class InputManager
{
    private readonly HashSet<Key> _pressedKeys = new();
    private readonly HashSet<Key> _justPressedKeys = new();
    private readonly HashSet<Key> _justReleasedKeys = new();

    private GamepadManager? _gamepad;

    public void SetGamepad(GamepadManager gamepad)
    {
        _gamepad = gamepad;
    }

    public bool IsKeyDown(Key key) => _pressedKeys.Contains(key);
    public bool IsKeyJustPressed(Key key) => _justPressedKeys.Contains(key);
    public bool IsKeyJustReleased(Key key) => _justReleasedKeys.Contains(key);

    // Combined keyboard + gamepad input
    public bool MoveLeft => IsKeyDown(Key.Left) || IsKeyDown(Key.A) || (_gamepad?.MoveLeft ?? false);
    public bool MoveRight => IsKeyDown(Key.Right) || IsKeyDown(Key.D) || (_gamepad?.MoveRight ?? false);
    public bool Fire => IsKeyDown(Key.Space) || IsKeyDown(Key.Z) || (_gamepad?.Fire ?? false);
    public bool FireJustPressed => IsKeyJustPressed(Key.Space) || IsKeyJustPressed(Key.Z) || (_gamepad?.FireJustPressed ?? false);
    public bool SuperZapper => IsKeyJustPressed(Key.Tab) || IsKeyJustPressed(Key.X) || (_gamepad?.SuperZapperJustPressed ?? false);
    public bool Pause => IsKeyJustPressed(Key.Escape) || IsKeyJustPressed(Key.P) || (_gamepad?.PauseJustPressed ?? false);
    public bool Start => IsKeyJustPressed(Key.Enter) || IsKeyJustPressed(Key.Space) || (_gamepad?.StartJustPressed ?? false);

    public bool GamepadConnected => _gamepad?.IsConnected ?? false;

    public void OnKeyDown(Key key)
    {
        if (!_pressedKeys.Contains(key))
        {
            _justPressedKeys.Add(key);
        }
        _pressedKeys.Add(key);
    }

    public void OnKeyUp(Key key)
    {
        _pressedKeys.Remove(key);
        _justReleasedKeys.Add(key);
    }

    public void Update()
    {
        _justPressedKeys.Clear();
        _justReleasedKeys.Clear();

        // Update gamepad state
        _gamepad?.Update();
    }
}
