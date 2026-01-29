namespace AVATempest.Audio;

public static class SoundGenerator
{
    private const int SampleRate = 44100;

    public static byte[] GenerateShootSound()
    {
        int duration = SampleRate / 15; // ~67ms
        var samples = new byte[duration * 2];

        for (int i = 0; i < duration; i++)
        {
            float t = i / (float)duration;
            float frequency = 800 - t * 400; // Descending pitch
            float amplitude = (1 - t) * 0.6f;

            double wave = Math.Sin(2 * Math.PI * frequency * i / SampleRate);
            short sample = (short)(wave * amplitude * short.MaxValue);

            samples[i * 2] = (byte)(sample & 0xFF);
            samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return samples;
    }

    public static byte[] GenerateExplosionSound()
    {
        int duration = SampleRate / 6; // ~167ms
        var samples = new byte[duration * 2];
        var random = new Random(42);

        float prevSample = 0;
        for (int i = 0; i < duration; i++)
        {
            float t = i / (float)duration;
            float amplitude = (1 - t) * 0.7f;

            // Low-pass filtered noise for explosion
            float noise = (float)(random.NextDouble() * 2 - 1);
            float filtered = prevSample * 0.7f + noise * 0.3f;
            prevSample = filtered;

            short sample = (short)(filtered * amplitude * short.MaxValue);
            samples[i * 2] = (byte)(sample & 0xFF);
            samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return samples;
    }

    public static byte[] GeneratePlayerDeathSound()
    {
        int duration = SampleRate / 2; // 500ms
        var samples = new byte[duration * 2];
        var random = new Random(123);

        for (int i = 0; i < duration; i++)
        {
            float t = i / (float)duration;
            float frequency = 400 - t * 350; // Descending low pitch
            float amplitude = (1 - t * 0.8f) * 0.6f;

            // Mix sine with noise
            double wave = Math.Sin(2 * Math.PI * frequency * i / SampleRate) * 0.6;
            wave += (random.NextDouble() * 2 - 1) * 0.4 * (1 - t);

            short sample = (short)(wave * amplitude * short.MaxValue);
            samples[i * 2] = (byte)(sample & 0xFF);
            samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return samples;
    }

    public static byte[] GenerateSuperZapperSound()
    {
        int duration = SampleRate / 3; // ~333ms
        var samples = new byte[duration * 2];

        for (int i = 0; i < duration; i++)
        {
            float t = i / (float)duration;
            float frequency = 150 + t * 1200; // Ascending pitch
            float amplitude = Math.Min(t * 3, 1 - t) * 0.7f;

            // Rich harmonics
            double wave = Math.Sin(2 * Math.PI * frequency * i / SampleRate) * 0.5 +
                          Math.Sin(4 * Math.PI * frequency * i / SampleRate) * 0.3 +
                          Math.Sin(6 * Math.PI * frequency * i / SampleRate) * 0.2;

            short sample = (short)(wave * amplitude * short.MaxValue);
            samples[i * 2] = (byte)(sample & 0xFF);
            samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return samples;
    }

    public static byte[] GenerateLevelCompleteSound()
    {
        int noteDuration = SampleRate / 5; // 200ms per note
        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
        var samples = new byte[noteDuration * notes.Length * 2];

        for (int n = 0; n < notes.Length; n++)
        {
            float frequency = notes[n];

            for (int i = 0; i < noteDuration; i++)
            {
                float t = i / (float)noteDuration;
                float amplitude = (1 - t * 0.5f) * 0.5f;

                double wave = Math.Sin(2 * Math.PI * frequency * i / SampleRate);
                short sample = (short)(wave * amplitude * short.MaxValue);

                int idx = (n * noteDuration + i) * 2;
                samples[idx] = (byte)(sample & 0xFF);
                samples[idx + 1] = (byte)((sample >> 8) & 0xFF);
            }
        }

        return samples;
    }

    public static byte[] GenerateWarpSound()
    {
        int duration = SampleRate; // 1 second
        var samples = new byte[duration * 2];

        for (int i = 0; i < duration; i++)
        {
            float t = i / (float)duration;
            float frequency = 100 + t * t * 2000; // Exponential pitch rise
            float amplitude = 0.4f * (1 - t * 0.5f);

            double wave = Math.Sin(2 * Math.PI * frequency * i / SampleRate);
            // Add some wobble
            wave *= 1 + 0.3 * Math.Sin(2 * Math.PI * 8 * t);

            short sample = (short)(wave * amplitude * short.MaxValue);
            samples[i * 2] = (byte)(sample & 0xFF);
            samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return samples;
    }
}
