using System.Numerics;
using AVATempest.Rendering;

namespace AVATempest.Levels;

public class LevelManager
{
    public int CurrentLevelNumber { get; private set; }
    public Level? CurrentLevel { get; private set; }

    private Vector2 _center;
    private float _innerRadius;
    private float _outerRadius;

    public void Initialize(Vector2 center, float innerRadius, float outerRadius)
    {
        _center = center;
        _innerRadius = innerRadius;
        _outerRadius = outerRadius;
    }

    public void UpdateDimensions(Vector2 center, float innerRadius, float outerRadius)
    {
        _center = center;
        _innerRadius = innerRadius;
        _outerRadius = outerRadius;

        // Regenerate current level tube with new dimensions
        if (CurrentLevel != null)
        {
            LoadLevel(CurrentLevelNumber);
        }
    }

    public Level LoadLevel(int levelNumber)
    {
        CurrentLevelNumber = levelNumber;

        var tube = TubeShapes.CreateLevel(levelNumber, _center, _innerRadius, _outerRadius);
        var color = ColorPalette.GetLevelColor(levelNumber);

        // Scale difficulty with level
        int baseFlippers = 4 + levelNumber;
        int baseTankers = Math.Max(0, (levelNumber - 2) / 2);
        int baseSpikers = Math.Max(0, (levelNumber - 1) / 2);
        int baseFuseballs = Math.Max(0, (levelNumber - 4) / 3);
        int basePulsars = Math.Max(0, (levelNumber - 3) / 3);

        // Cap enemies at reasonable levels
        int flippers = Math.Min(baseFlippers, 20);
        int tankers = Math.Min(baseTankers, 8);
        int spikers = Math.Min(baseSpikers, 6);
        int fuseballs = Math.Min(baseFuseballs, 4);
        int pulsars = Math.Min(basePulsars, 4);

        CurrentLevel = new Level(levelNumber, tube, color,
            flippers, tankers, spikers, fuseballs, pulsars);

        return CurrentLevel;
    }

    public Level NextLevel()
    {
        return LoadLevel(CurrentLevelNumber + 1);
    }

    public void Reset()
    {
        CurrentLevelNumber = 0;
        CurrentLevel = null;
    }
}
