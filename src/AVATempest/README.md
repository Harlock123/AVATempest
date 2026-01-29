# AVATempest

A cross-platform recreation of the classic 1981 Atari arcade game **Tempest**, built with .NET 9 and Avalonia using SkiaSharp for vector graphics rendering.

## Features

- Classic tube-based gameplay with 8 unique level shapes
- All original enemy types: Flippers, Tankers, Spikers, Fuseballs, and Pulsars
- Glowing vector graphics with particle effects
- Synthesized retro audio
- Super Zapper weapon (2 charges per level)
- Progressive difficulty with color-cycling levels
- High score persistence

## Controls

| Key | Action |
|-----|--------|
| LEFT/RIGHT or A/D | Move around tube edge |
| SPACE | Fire |
| TAB | Super Zapper |
| ESC | Pause |
| ENTER | Start game |

## Requirements

- .NET 9.0 SDK
- Linux, Windows, or macOS

## Build & Run

```bash
dotnet build
dotnet run
```

## Project Structure

```
AVATempest/
├── Core/           # Game engine, input, collision detection
├── Entities/       # Player, projectiles, enemies
├── Levels/         # Tube geometry and level management
├── Rendering/      # SkiaSharp vector graphics rendering
├── Audio/          # Synthesized sound effects
└── UI/             # Game canvas, attract mode, high scores
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Lonnie Watson
