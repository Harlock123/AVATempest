# AVATempest

A cross-platform recreation of the classic 1981 Atari arcade game **Tempest**, built with .NET 9 and Avalonia using SkiaSharp for vector graphics rendering.

## Features

- Classic tube-based gameplay with 8 unique level shapes (Circle, Square, Plus, Triangle, Star, V, Clover, Heart)
- All original enemy types with authentic behaviors:
  - **Flippers** - Move up the tube and flip between lanes at the top
  - **Tankers** - Slow movers that split into two Flippers when destroyed
  - **Spikers** - Leave dangerous spike trails on the tube
  - **Fuseballs** - Bounce along the tube rim
  - **Pulsars** - Electrify tube segments periodically
- Glowing vector graphics with particle effects
- Synthesized retro audio (shoot, explosions, level complete, warp)
- Super Zapper weapon (2 charges per level - first use kills all enemies, second kills one)
- Progressive difficulty with color-cycling levels
- High score persistence (saved to local app data)
- Gamepad/controller support (auto-detected)
- Bonus life every 20,000 points

## Controls

### Keyboard

| Key | Action |
|-----|--------|
| LEFT/RIGHT or A/D | Move around tube edge |
| SPACE or Z | Fire |
| TAB or X | Super Zapper |
| ESC or P | Pause |
| ENTER or SPACE | Start game / Resume / Play again |
| ESC (while paused) | Quit game |
| ESC (on Game Over) | Quit game |

### Controller (Xbox/PlayStation/Generic)

| Button | Action |
|--------|--------|
| Left Stick / D-Pad | Move around tube edge |
| A / Cross | Fire |
| Right Trigger | Fire (alternate) |
| X / Square / LB | Super Zapper |
| Start | Start game / Pause |

## Scoring

| Enemy | Points |
|-------|--------|
| Flipper | 150 |
| Tanker | 250 |
| Spiker | 50 |
| Fuseball | 750 |
| Pulsar | 200 |

**Bonus:** Extra life every 20,000 points

## Requirements

- .NET 9.0 SDK
- Linux, Windows, or macOS
- SDL2 (for controller support - included via NuGet)

## Build & Run

```bash
cd src/AVATempest
dotnet build
dotnet run
```

## Project Structure

```
AVATempest/
├── Core/           # Game engine, input, collision detection, gamepad
├── Entities/       # Player, projectiles, enemies
├── Levels/         # Tube geometry and level management
├── Rendering/      # SkiaSharp vector graphics rendering
├── Audio/          # Synthesized sound effects
└── UI/             # Game canvas, attract mode, high scores
```

## Technologies

- **.NET 9** - Runtime and SDK
- **Avalonia 11** - Cross-platform UI framework
- **SkiaSharp** - 2D graphics rendering
- **Silk.NET.SDL** - Gamepad/controller input
- **NetCoreAudio** - Cross-platform audio playback

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Lonnie Watson
