using Avalonia.Input;

namespace AVATempest.Core;

public class InputManager
{
    private readonly HashSet<Key> _pressedKeys = new();
    private readonly HashSet<Key> _justPressedKeys = new();
    private readonly HashSet<Key> _justReleasedKeys = new();

    public bool IsKeyDown(Key key) => _pressedKeys.Contains(key);
    public bool IsKeyJustPressed(Key key) => _justPressedKeys.Contains(key);
    public bool IsKeyJustReleased(Key key) => _justReleasedKeys.Contains(key);

    public bool MoveLeft => IsKeyDown(Key.Left) || IsKeyDown(Key.A);
    public bool MoveRight => IsKeyDown(Key.Right) || IsKeyDown(Key.D);
    public bool Fire => IsKeyDown(Key.Space) || IsKeyDown(Key.Z);
    public bool FireJustPressed => IsKeyJustPressed(Key.Space) || IsKeyJustPressed(Key.Z);
    public bool SuperZapper => IsKeyJustPressed(Key.Tab) || IsKeyJustPressed(Key.X);
    public bool Pause => IsKeyJustPressed(Key.Escape) || IsKeyJustPressed(Key.P);
    public bool Start => IsKeyJustPressed(Key.Enter) || IsKeyJustPressed(Key.Space);

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
    }
}
