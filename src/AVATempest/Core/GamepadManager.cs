using Silk.NET.SDL;

namespace AVATempest.Core;

public unsafe class GamepadManager : IDisposable
{
    private readonly Sdl _sdl;
    private GameController* _controller;
    private bool _initialized;

    // Current state
    public bool MoveLeft { get; private set; }
    public bool MoveRight { get; private set; }
    public bool Fire { get; private set; }
    public bool FireJustPressed { get; private set; }
    public bool SuperZapper { get; private set; }
    public bool Pause { get; private set; }
    public bool Start { get; private set; }

    // Previous frame state for edge detection
    private bool _prevFire;
    private bool _prevSuperZapper;
    private bool _prevPause;
    private bool _prevStart;

    private const float DeadZone = 0.3f;

    public bool IsConnected => _controller != null;

    public GamepadManager()
    {
        _sdl = Sdl.GetApi();

        try
        {
            // Initialize SDL with gamecontroller subsystem
            if (_sdl.Init(Sdl.InitGamecontroller | Sdl.InitJoystick) < 0)
            {
                return;
            }

            _initialized = true;

            // Try to open the first available controller
            OpenController();
        }
        catch
        {
            // SDL initialization failed, continue without gamepad
        }
    }

    private void OpenController()
    {
        if (!_initialized) return;

        int numJoysticks = _sdl.NumJoysticks();
        for (int i = 0; i < numJoysticks; i++)
        {
            if (_sdl.IsGameController(i) == SdlBool.True)
            {
                _controller = _sdl.GameControllerOpen(i);
                if (_controller != null)
                {
                    break;
                }
            }
        }
    }

    public void Update()
    {
        if (!_initialized) return;

        // Pump SDL events
        _sdl.PumpEvents();

        // Check if controller disconnected
        if (_controller != null && _sdl.GameControllerGetAttached(_controller) == SdlBool.False)
        {
            _sdl.GameControllerClose(_controller);
            _controller = null;
        }

        // Try to reconnect if no controller
        if (_controller == null)
        {
            OpenController();
        }

        // Store previous state
        _prevFire = Fire;
        _prevSuperZapper = SuperZapper;
        _prevPause = Pause;
        _prevStart = Start;

        // Reset state
        MoveLeft = false;
        MoveRight = false;
        Fire = false;
        SuperZapper = false;
        Pause = false;
        Start = false;

        if (_controller == null) return;

        // Read left stick X axis
        short leftX = _sdl.GameControllerGetAxis(_controller, GameControllerAxis.Leftx);
        float normalizedX = leftX / 32767f;

        if (normalizedX < -DeadZone)
            MoveLeft = true;
        else if (normalizedX > DeadZone)
            MoveRight = true;

        // Read D-pad
        if (_sdl.GameControllerGetButton(_controller, GameControllerButton.DpadLeft) == 1)
            MoveLeft = true;
        if (_sdl.GameControllerGetButton(_controller, GameControllerButton.DpadRight) == 1)
            MoveRight = true;

        // A button = Fire
        Fire = _sdl.GameControllerGetButton(_controller, GameControllerButton.A) == 1;

        // Also check right trigger for fire
        short rightTrigger = _sdl.GameControllerGetAxis(_controller, GameControllerAxis.Triggerright);
        if (rightTrigger > 8000)
            Fire = true;

        // X or Left Bumper = Super Zapper
        SuperZapper = _sdl.GameControllerGetButton(_controller, GameControllerButton.X) == 1 ||
                      _sdl.GameControllerGetButton(_controller, GameControllerButton.Leftshoulder) == 1;

        // Start button = Start/Pause
        Start = _sdl.GameControllerGetButton(_controller, GameControllerButton.Start) == 1;
        Pause = Start;

        // Calculate "just pressed" states
        FireJustPressed = Fire && !_prevFire;
    }

    public bool SuperZapperJustPressed => SuperZapper && !_prevSuperZapper;
    public bool PauseJustPressed => Pause && !_prevPause;
    public bool StartJustPressed => Start && !_prevStart;

    public void Dispose()
    {
        if (_controller != null)
        {
            _sdl.GameControllerClose(_controller);
            _controller = null;
        }

        if (_initialized)
        {
            _sdl.QuitSubSystem(Sdl.InitGamecontroller | Sdl.InitJoystick);
        }
    }
}
