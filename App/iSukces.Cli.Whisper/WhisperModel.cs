namespace iSukces.Cli.Whisper;

/// <summary>
/// Model metadata aligned with the public OpenAI Whisper model table.
/// </summary>
public sealed class WhisperModel : IEquatable<WhisperModel>
{
    public static implicit operator WhisperModelName(WhisperModel? src)
        => src?.ModelName ?? WhisperModelName.Empty;

    public override string ToString()
    {
        return $"{ModelName.Value} ({Parameters}M params, ~{Required_VRAM_GB} GB VRAM)";
    }

    public bool Equals(WhisperModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModelName.Equals(other.ModelName);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is WhisperModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ModelName.GetHashCode();
    }

    public static bool operator ==(WhisperModel? left, WhisperModel? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(WhisperModel? left, WhisperModel? right)
    {
        return !Equals(left, right);
    }

    public required WhisperModelName ModelName          { get; init; }
    public required int              Parameters         { get; init; } // w milionach
    public          string?          English_only_model { get; init; }
    public required string           Multilingual_model { get; init; }
    public required int              Required_VRAM_GB   { get; init; }
    public required int              Relative_speed     { get; init; } // mnożnik (np. 10 = ~10x)

    public static IEnumerable<WhisperModel> All => new[]
    {
        Tiny,
        Base,
        Small,
        Medium,
        Large,
        Turbo
    };

    public static WhisperModel Tiny => new()
    {
        ModelName          = new WhisperModelName("tiny"),
        Parameters         = 39,
        English_only_model = "tiny.en",
        Multilingual_model = "tiny",
        Required_VRAM_GB   = 1,
        Relative_speed     = 10
    };

    public static WhisperModel Base => new()
    {
        ModelName          = new WhisperModelName("base"),
        Parameters         = 74,
        English_only_model = "base.en",
        Multilingual_model = "base",
        Required_VRAM_GB   = 1,
        Relative_speed     = 7
    };

    public static WhisperModel Small => new()
    {
        ModelName          = new WhisperModelName("small"),
        Parameters         = 244,
        English_only_model = "small.en",
        Multilingual_model = "small",
        Required_VRAM_GB   = 2,
        Relative_speed     = 4
    };

    public static WhisperModel Medium => new()
    {
        ModelName          = new WhisperModelName("medium"),
        Parameters         = 769,
        English_only_model = "medium.en",
        Multilingual_model = "medium",
        Required_VRAM_GB   = 5,
        Relative_speed     = 2
    };

    public static WhisperModel Large => new()
    {
        ModelName          = new WhisperModelName("large"),
        Parameters         = 1550,
        English_only_model = null,
        Multilingual_model = "large",
        Required_VRAM_GB   = 10,
        Relative_speed     = 1
    };

    public static WhisperModel Turbo => new()
    {
        ModelName          = new WhisperModelName("turbo"),
        Parameters         = 809,
        English_only_model = null,
        Multilingual_model = "turbo",
        Required_VRAM_GB   = 6,
        Relative_speed     = 8
    };
}
