using AVATempest.Entities.Enemies;
using SkiaSharp;

namespace AVATempest.Levels;

public class Level
{
    public int LevelNumber { get; }
    public Tube Tube { get; }
    public SKColor PrimaryColor { get; }
    public int TotalEnemies { get; }
    public int FlipperCount { get; }
    public int TankerCount { get; }
    public int SpikerCount { get; }
    public int FuseballCount { get; }
    public int PulsarCount { get; }

    public Level(int levelNumber, Tube tube, SKColor color,
        int flippers, int tankers, int spikers, int fuseballs, int pulsars)
    {
        LevelNumber = levelNumber;
        Tube = tube;
        PrimaryColor = color;
        FlipperCount = flippers;
        TankerCount = tankers;
        SpikerCount = spikers;
        FuseballCount = fuseballs;
        PulsarCount = pulsars;
        TotalEnemies = flippers + tankers + spikers + fuseballs + pulsars;
    }

    public List<Enemy> CreateEnemyWave(int waveNumber, int maxActive)
    {
        var enemies = new List<Enemy>();
        var random = Random.Shared;

        // Distribute enemies across waves
        int remainingFlippers = Math.Max(0, FlipperCount - waveNumber * 2);
        int remainingTankers = Math.Max(0, TankerCount - waveNumber);
        int remainingSpikers = Math.Max(0, SpikerCount - waveNumber);
        int remainingFuseballs = waveNumber >= 2 ? Math.Min(FuseballCount, waveNumber - 1) : 0;
        int remainingPulsars = waveNumber >= 1 ? Math.Min(PulsarCount, waveNumber) : 0;

        int toSpawn = Math.Min(maxActive, remainingFlippers + remainingTankers + remainingSpikers);

        for (int i = 0; i < toSpawn; i++)
        {
            Enemy enemy;
            int type = random.Next(100);

            if (type < 50 && remainingFlippers > 0)
            {
                enemy = new Flipper();
                remainingFlippers--;
            }
            else if (type < 70 && remainingTankers > 0)
            {
                enemy = new Tanker();
                remainingTankers--;
            }
            else if (type < 85 && remainingSpikers > 0)
            {
                enemy = new Spiker();
                remainingSpikers--;
            }
            else if (type < 95 && remainingPulsars > 0)
            {
                enemy = new Pulsar();
                remainingPulsars--;
            }
            else if (remainingFuseballs > 0)
            {
                enemy = new Fuseball();
                remainingFuseballs--;
            }
            else if (remainingFlippers > 0)
            {
                enemy = new Flipper();
                remainingFlippers--;
            }
            else
            {
                continue;
            }

            enemy.SegmentIndex = random.Next(Tube.SegmentCount);
            enemy.Depth = random.NextSingle() * 0.2f; // Spawn near center
            enemies.Add(enemy);
        }

        return enemies;
    }
}
