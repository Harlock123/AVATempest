# AGENTS.md

Notes for anyone, human or AI agent, changing this code. `README.md` covers playing the game, its controls and
scoring; this file covers how the code is put together and what to watch when changing it.

## What this is

A recreation of Atari's 1981 vector arcade game Tempest, in C# on .NET 9 and Avalonia 11, drawn with SkiaSharp.
It is a single project with no tests. Sound effects are synthesised in code and played through NetCoreAudio;
gamepads come through SDL2 (Silk.NET).

## Commands

```bash
dotnet build AVATempest.sln                    # targets net9.0; there is no global.json
dotnet run --project src/AVATempest            # play it
```

- There are no tests yet.
- The program takes no command-line arguments. Ctrl+S in the game saves a screenshot to
  `SpecialFolder.MyPictures`/`AVATempest`; the pictures in `Images/` were made this way.

## Layout

- `src/AVATempest/MainWindow.axaml.cs`: owns the game loop (a 16 ms `DispatcherTimer`), passes keys to the
  input manager and takes screenshots.
- `src/AVATempest/Core/`:
  - `GameEngine.cs`: the state machine (attract, playing, level complete, warping, dying, game over), spawning
    and scoring.
  - `GameState.cs`, `CollisionDetection.cs`.
  - `InputManager.cs` (keys, mouse wheel) and `GamepadManager.cs` (SDL2 game controllers).
- `src/AVATempest/Entities/`: `Player`, `Projectile`, `SuperZapper`, and `Enemies/` (Flipper, Tanker, Spiker,
  Fuseball, Pulsar, all derived from `Enemy`).
- `src/AVATempest/Levels/`: `Tube`, `TubeSegment` and `TubeShapes` (the web geometry), `Level`, `LevelManager`.
- `src/AVATempest/Rendering/`: `GameRenderer` with tube, entity, HUD, particle and vector renderers, and
  `ColorPalette`.
- `src/AVATempest/Audio/`: `SoundEffects.cs` generates the PCM data; `AudioManager` writes it as WAV files into a
  temp folder and plays them.
- `src/AVATempest/UI/`: `GameCanvas` (the drawing surface), `AttractMode`, `HighScoreManager`, `HighScoreScreen`.

## Rules that keep it working

- **Variable time step.** `GameEngine.Update(deltaTime)` takes the real elapsed time from a `Stopwatch`, capped at
  0.1 s. Movement and timers must scale by `deltaTime`. There is no fixed-step loop.
- **Randomness is not seeded.** Gameplay uses `Random.Shared`, so runs are not repeatable; only the sound
  synthesis uses fixed seeds (`new Random(42)` and `new Random(123)`), so the sounds are the same every run.
- **Input.** Movement round the tube is left/right only (`MoveLeft`, `MoveRight`), from arrow keys, A/D, the
  mouse wheel or a gamepad. Keys are tracked by Avalonia `Key`.
- **High scores** are saved as `highscores.json` in `SpecialFolder.LocalApplicationData`/`AVATempest`.
- **Audio** must never block the game: sounds play on background tasks, and playback errors are caught and ignored.

## Making a change

- Build with no new warnings, and play the change by hand, since there are no automated tests.
- If the change shows on screen, take new screenshots with Ctrl+S, copy them into `Images/`, and update
  `README.md` (features, controls, scoring) to match.

## CI and releases

`.github/workflows/build.yml` builds the solution on every push and pull request, on .NET 9. It then publishes
self-contained single-file builds for win-x64/x86/arm64, linux-x64/arm64/arm and osx-x64/arm64, and keeps them as
workflow artifacts. Pushes to `master` (this repo's default branch) replace the assets of the `latest`
pre-release, and a `v*` tag makes a proper release.
