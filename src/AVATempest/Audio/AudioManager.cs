using NetCoreAudio;

namespace AVATempest.Audio;

public enum SoundEffect
{
    Shoot,
    EnemyDeath,
    PlayerDeath,
    LevelComplete,
    SuperZapper,
    Warp
}

public class AudioManager : IDisposable
{
    private readonly Dictionary<SoundEffect, string> _soundFiles = new();
    private readonly Dictionary<SoundEffect, Player> _players = new();
    private readonly string _soundDir;
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    public AudioManager()
    {
        _soundDir = Path.Combine(Path.GetTempPath(), "AVATempest_Audio");
        Directory.CreateDirectory(_soundDir);

        // Generate all sound effects as WAV files
        GenerateSoundFiles();

        // Create players for each sound
        foreach (var effect in Enum.GetValues<SoundEffect>())
        {
            _players[effect] = new Player();
        }
    }

    private void GenerateSoundFiles()
    {
        _soundFiles[SoundEffect.Shoot] = GenerateWavFile("shoot.wav", SoundGenerator.GenerateShootSound());
        _soundFiles[SoundEffect.EnemyDeath] = GenerateWavFile("enemy_death.wav", SoundGenerator.GenerateExplosionSound());
        _soundFiles[SoundEffect.PlayerDeath] = GenerateWavFile("player_death.wav", SoundGenerator.GeneratePlayerDeathSound());
        _soundFiles[SoundEffect.LevelComplete] = GenerateWavFile("level_complete.wav", SoundGenerator.GenerateLevelCompleteSound());
        _soundFiles[SoundEffect.SuperZapper] = GenerateWavFile("super_zapper.wav", SoundGenerator.GenerateSuperZapperSound());
        _soundFiles[SoundEffect.Warp] = GenerateWavFile("warp.wav", SoundGenerator.GenerateWarpSound());
    }

    private string GenerateWavFile(string filename, byte[] pcmData)
    {
        string path = Path.Combine(_soundDir, filename);

        using var fs = new FileStream(path, FileMode.Create);
        using var writer = new BinaryWriter(fs);

        int sampleRate = 44100;
        short bitsPerSample = 16;
        short channels = 1;
        int byteRate = sampleRate * channels * bitsPerSample / 8;
        short blockAlign = (short)(channels * bitsPerSample / 8);

        // RIFF header
        writer.Write("RIFF"u8);
        writer.Write(36 + pcmData.Length); // File size - 8
        writer.Write("WAVE"u8);

        // fmt chunk
        writer.Write("fmt "u8);
        writer.Write(16); // Chunk size
        writer.Write((short)1); // Audio format (PCM)
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);

        // data chunk
        writer.Write("data"u8);
        writer.Write(pcmData.Length);
        writer.Write(pcmData);

        return path;
    }

    public void PlaySound(SoundEffect effect)
    {
        if (!_enabled) return;

        try
        {
            if (_soundFiles.TryGetValue(effect, out var file) && _players.TryGetValue(effect, out var player))
            {
                // Play asynchronously without blocking
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await player.Play(file);
                    }
                    catch
                    {
                        // Ignore playback errors
                    }
                });
            }
        }
        catch
        {
            // Silently ignore audio errors
        }
    }

    public void StopAll()
    {
        foreach (var player in _players.Values)
        {
            try
            {
                player.Stop();
            }
            catch
            {
                // Ignore
            }
        }
    }

    public void Dispose()
    {
        StopAll();

        // Clean up temp files
        try
        {
            if (Directory.Exists(_soundDir))
            {
                Directory.Delete(_soundDir, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
