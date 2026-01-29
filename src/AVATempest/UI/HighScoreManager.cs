using System.Text.Json;

namespace AVATempest.UI;

public class HighScoreEntry
{
    public string Name { get; set; } = "AAA";
    public int Score { get; set; }
    public int Level { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}

public class HighScoreManager
{
    private const int MaxEntries = 10;
    private readonly string _filePath;
    private List<HighScoreEntry> _entries = new();

    public IReadOnlyList<HighScoreEntry> Entries => _entries;
    public int TopScore => _entries.Count > 0 ? _entries[0].Score : 0;

    public HighScoreManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var gameDir = Path.Combine(appData, "AVATempest");
        Directory.CreateDirectory(gameDir);
        _filePath = Path.Combine(gameDir, "highscores.json");

        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _entries = JsonSerializer.Deserialize<List<HighScoreEntry>>(json) ?? new List<HighScoreEntry>();
            }
        }
        catch
        {
            _entries = new List<HighScoreEntry>();
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // Silently fail if we can't save
        }
    }

    public bool IsHighScore(int score)
    {
        if (_entries.Count < MaxEntries)
            return score > 0;

        return score > _entries[^1].Score;
    }

    public int AddScore(string name, int score, int level)
    {
        var entry = new HighScoreEntry
        {
            Name = name.ToUpperInvariant().PadRight(3)[..3],
            Score = score,
            Level = level,
            Date = DateTime.Now
        };

        _entries.Add(entry);
        _entries = _entries.OrderByDescending(e => e.Score).Take(MaxEntries).ToList();
        Save();

        return _entries.IndexOf(entry);
    }

    public void Reset()
    {
        _entries.Clear();
        Save();
    }
}
