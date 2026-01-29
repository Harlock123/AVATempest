using System.Numerics;
using AVATempest.Audio;
using AVATempest.Entities;
using AVATempest.Entities.Enemies;
using AVATempest.Levels;
using AVATempest.Rendering;

namespace AVATempest.Core;

public class GameEngine
{
    public GameState State { get; private set; } = GameState.Attract;
    public float StateProgress { get; private set; }

    public Player Player { get; } = new();
    public SuperZapper SuperZapper { get; } = new();
    public List<Enemy> Enemies { get; } = new();
    public List<Projectile> Projectiles { get; } = new();
    public LevelManager LevelManager { get; } = new();
    public InputManager Input { get; } = new();

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Lives { get; private set; } = 3;
    public int LevelBonus { get; private set; }
    public float GameTime { get; private set; }

    private float _spawnTimer;
    private float _spawnInterval = 2f;
    private int _currentWave;
    private int _enemiesSpawned;
    private int _enemiesKilled;

    private float _stateTimer;
    private const float LevelCompleteDelay = 3f;
    private const float DeathDelay = 2f;
    private const float WarpDelay = 1.5f;

    private ParticleSystem? _particleSystem;
    private AudioManager? _audioManager;

    public void Initialize(Vector2 center, float innerRadius, float outerRadius)
    {
        LevelManager.Initialize(center, innerRadius, outerRadius);
        LevelManager.LoadLevel(0);
        State = GameState.Attract;
    }

    public void SetParticleSystem(ParticleSystem particleSystem)
    {
        _particleSystem = particleSystem;
    }

    public void SetAudioManager(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void UpdateDimensions(Vector2 center, float innerRadius, float outerRadius)
    {
        LevelManager.UpdateDimensions(center, innerRadius, outerRadius);
    }

    public void Update(float deltaTime)
    {
        GameTime += deltaTime;

        switch (State)
        {
            case GameState.Attract:
                UpdateAttract(deltaTime);
                break;
            case GameState.Playing:
                UpdatePlaying(deltaTime);
                break;
            case GameState.LevelComplete:
                UpdateLevelComplete(deltaTime);
                break;
            case GameState.Warping:
                UpdateWarping(deltaTime);
                break;
            case GameState.Dying:
                UpdateDying(deltaTime);
                break;
            case GameState.GameOver:
                UpdateGameOver(deltaTime);
                break;
            case GameState.Paused:
                // Don't update anything when paused
                break;
        }

        Input.Update();
    }

    private void UpdateAttract(float deltaTime)
    {
        _particleSystem?.Update(deltaTime);

        // Random particle effects in attract mode
        if (Random.Shared.NextSingle() < 0.05f)
        {
            var level = LevelManager.CurrentLevel;
            if (level != null)
            {
                var segment = level.Tube.Segments[Random.Shared.Next(level.Tube.SegmentCount)];
                _particleSystem?.SpawnLine(segment.InnerLeft, segment.OuterLeft, level.PrimaryColor, 5);
            }
        }

        if (Input.Start)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Score = 0;
        Lives = 3;
        _currentWave = 0;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;

        LevelManager.LoadLevel(0);
        ResetLevel();
        State = GameState.Playing;
    }

    private void ResetLevel()
    {
        Enemies.Clear();
        Projectiles.Clear();
        _particleSystem?.Clear();

        Player.IsAlive = true;
        Player.SegmentIndex = 0;
        Player.ResetForLevel();

        var level = LevelManager.CurrentLevel!;
        level.Tube.ClearSpikes();
        level.Tube.ClearElectrification();

        _spawnTimer = 0;
        _spawnInterval = Math.Max(0.5f, 2f - LevelManager.CurrentLevelNumber * 0.1f);
        _currentWave = 0;
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
    }

    private void UpdatePlaying(float deltaTime)
    {
        if (Input.Pause)
        {
            State = GameState.Paused;
            return;
        }

        var level = LevelManager.CurrentLevel;
        if (level == null) return;

        var tube = level.Tube;

        // Update player movement
        int moveDir = 0;
        if (Input.MoveLeft) moveDir -= 1;
        if (Input.MoveRight) moveDir += 1;
        if (moveDir != 0)
        {
            Player.Move(moveDir, tube, deltaTime);
        }

        // Player shooting
        if (Input.Fire)
        {
            var projectile = Player.Fire(tube);
            if (projectile != null)
            {
                Projectiles.Add(projectile);
                _audioManager?.PlaySound(SoundEffect.Shoot);
            }
        }

        // Super Zapper
        if (Input.SuperZapper && Player.CanUseSuperZapper())
        {
            Player.UseSuperZapper();
            SuperZapper.Activate();
            ActivateSuperZapper();
            _audioManager?.PlaySound(SoundEffect.SuperZapper);
        }

        // Update entities
        Player.Update(deltaTime, tube);
        SuperZapper.Update(deltaTime);

        UpdateProjectiles(deltaTime, tube);
        UpdateEnemies(deltaTime, tube);
        SpawnEnemies(deltaTime, level);

        // Update particles
        _particleSystem?.Update(deltaTime);

        // Check for level complete
        if (_enemiesKilled >= level.TotalEnemies && Enemies.Count == 0)
        {
            StartLevelComplete();
        }
    }

    private void UpdateProjectiles(float deltaTime, Tube tube)
    {
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update(deltaTime, tube);
            if (!Projectiles[i].IsAlive)
            {
                Projectiles.RemoveAt(i);
            }
        }
    }

    private void UpdateEnemies(float deltaTime, Tube tube)
    {
        var newEnemies = new List<Enemy>();

        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            var enemy = Enemies[i];
            enemy.UpdateBehavior(deltaTime, tube, Player);

            // Check collision with player
            if (!Player.IsInvulnerable && enemy.IsAtTop && enemy.SegmentIndex == Player.SegmentIndex)
            {
                PlayerDeath();
                return;
            }

            // Check collision with player projectiles
            bool hit = false;
            for (int j = Projectiles.Count - 1; j >= 0; j--)
            {
                var proj = Projectiles[j];
                if (proj.IsPlayerShot && proj.CollidesWith(enemy, tube, 20f))
                {
                    hit = true;
                    Projectiles.RemoveAt(j);
                    break;
                }
            }

            if (hit)
            {
                var pos = enemy.GetPosition(tube);
                _particleSystem?.SpawnExplosion(pos, enemy.Color, 15);
                _audioManager?.PlaySound(SoundEffect.EnemyDeath);
                Score += enemy.Points;
                _enemiesKilled++;

                var spawned = enemy.OnDestroyed(tube);
                newEnemies.AddRange(spawned);

                if (Score > HighScore)
                    HighScore = Score;

                // Bonus life every 20,000 points
                if (Score / 20000 > (Score - enemy.Points) / 20000)
                {
                    Lives++;
                }
            }

            if (!enemy.IsAlive)
            {
                Enemies.RemoveAt(i);
            }
        }

        Enemies.AddRange(newEnemies);

        // Check for spike collision during player movement
        var playerSegment = tube.GetSegment(Player.SegmentIndex);
        if (playerSegment.HasSpike && playerSegment.SpikeDepth > 0.9f && !Player.IsInvulnerable)
        {
            PlayerDeath();
            return;
        }

        // Check electrification
        if (playerSegment.IsElectrified && !Player.IsInvulnerable)
        {
            PlayerDeath();
        }
    }

    private void SpawnEnemies(float deltaTime, Level level)
    {
        if (_enemiesSpawned >= level.TotalEnemies) return;

        _spawnTimer += deltaTime;
        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer = 0;

            int maxActive = 5 + LevelManager.CurrentLevelNumber;
            if (Enemies.Count < maxActive)
            {
                var wave = level.CreateEnemyWave(_currentWave, Math.Min(3, maxActive - Enemies.Count));
                foreach (var enemy in wave)
                {
                    if (_enemiesSpawned < level.TotalEnemies)
                    {
                        Enemies.Add(enemy);
                        _enemiesSpawned++;
                    }
                }
                _currentWave++;
            }
        }
    }

    private void ActivateSuperZapper()
    {
        // Kill all enemies on first use, random enemy on subsequent uses per level
        if (Player.SuperZapperCharges == 1)
        {
            // Second use - kill one random enemy
            if (Enemies.Count > 0)
            {
                var enemy = Enemies[Random.Shared.Next(Enemies.Count)];
                var pos = enemy.GetPosition(LevelManager.CurrentLevel!.Tube);
                _particleSystem?.SpawnExplosion(pos, enemy.Color, 20);
                Score += enemy.Points;
                _enemiesKilled++;
                enemy.IsAlive = false;
            }
        }
        else
        {
            // First use - kill all visible enemies
            foreach (var enemy in Enemies)
            {
                var pos = enemy.GetPosition(LevelManager.CurrentLevel!.Tube);
                _particleSystem?.SpawnExplosion(pos, enemy.Color, 20);
                Score += enemy.Points;
                _enemiesKilled++;
                enemy.IsAlive = false;
            }
        }

        Enemies.RemoveAll(e => !e.IsAlive);
    }

    private void PlayerDeath()
    {
        var pos = Player.GetPosition(LevelManager.CurrentLevel!.Tube);
        _particleSystem?.SpawnExplosion(pos, Player.Color, 30);
        _audioManager?.PlaySound(SoundEffect.PlayerDeath);

        Player.IsAlive = false;
        Lives--;

        if (Lives <= 0)
        {
            State = GameState.GameOver;
            _stateTimer = 0;
        }
        else
        {
            State = GameState.Dying;
            _stateTimer = 0;
        }
    }

    private void StartLevelComplete()
    {
        State = GameState.LevelComplete;
        _stateTimer = 0;
        _audioManager?.PlaySound(SoundEffect.LevelComplete);

        // Calculate bonus based on remaining Super Zapper charges and spikes
        int spikeBonus = 0;
        var tube = LevelManager.CurrentLevel!.Tube;
        foreach (var segment in tube.Segments)
        {
            if (!segment.HasSpike)
                spikeBonus += 50;
        }

        LevelBonus = Player.SuperZapperCharges * 1000 + spikeBonus;
        Score += LevelBonus;

        if (Score > HighScore)
            HighScore = Score;
    }

    private void UpdateLevelComplete(float deltaTime)
    {
        _stateTimer += deltaTime;
        StateProgress = _stateTimer / LevelCompleteDelay;

        _particleSystem?.Update(deltaTime);

        if (_stateTimer >= LevelCompleteDelay)
        {
            State = GameState.Warping;
            _stateTimer = 0;
            _audioManager?.PlaySound(SoundEffect.Warp);

            var tube = LevelManager.CurrentLevel!.Tube;
            _particleSystem?.SpawnWarpEffect(tube.Center, tube.OuterRadius, LevelManager.CurrentLevel!.PrimaryColor);
        }
    }

    private void UpdateWarping(float deltaTime)
    {
        _stateTimer += deltaTime;
        StateProgress = _stateTimer / WarpDelay;

        _particleSystem?.Update(deltaTime);

        if (_stateTimer >= WarpDelay)
        {
            LevelManager.NextLevel();
            ResetLevel();
            State = GameState.Playing;
        }
    }

    private void UpdateDying(float deltaTime)
    {
        _stateTimer += deltaTime;

        _particleSystem?.Update(deltaTime);

        if (_stateTimer >= DeathDelay)
        {
            Player.IsAlive = true;
            Player.MakeInvulnerable(2f);
            Player.SegmentIndex = 0;
            State = GameState.Playing;
        }
    }

    private void UpdateGameOver(float deltaTime)
    {
        _particleSystem?.Update(deltaTime);

        if (Input.Start)
        {
            StartGame();
        }
    }

    public void TogglePause()
    {
        if (State == GameState.Playing)
            State = GameState.Paused;
        else if (State == GameState.Paused)
            State = GameState.Playing;
    }
}
