namespace iSukces.Cli.Whisper;

/// <summary>
/// Model metadata aligned with the public OpenAI Whisper model table.
/// </summary>
public sealed class WhisperModel : IEquatable<WhisperModel>
{
    /// <summary>
    /// Converts model metadata to its model name.
    /// </summary>
    /// <param name="src">Model metadata to convert.</param>
    /// <returns>The model name, or an empty model name when <paramref name="src"/> is <see langword="null"/>.</returns>
    public static implicit operator WhisperModelName(WhisperModel? src)
        => src?.ModelName ?? WhisperModelName.Empty;

    /// <summary>
    /// Text representation of the model metadata.
    /// </summary>
    /// <returns>A display text containing the model name, parameter count, and VRAM requirement.</returns>
    public override string ToString()
    {
        return $"{ModelName.Value} ({Parameters}M params, ~{Required_VRAM_GB} GB VRAM)";
    }

    /// <summary>
    /// Determines whether this model metadata represents the same model as another instance.
    /// </summary>
    /// <param name="other">Model metadata to compare with this instance.</param>
    /// <returns><see langword="true"/> when both instances represent the same model; otherwise, <see langword="false"/>.</returns>
    public bool Equals(WhisperModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ModelName.Equals(other.ModelName);
    }

    /// <summary>
    /// Determines whether this model metadata equals another object.
    /// </summary>
    /// <param name="obj">Object to compare with this instance.</param>
    /// <returns><see langword="true"/> when the object represents the same model; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is WhisperModel other && Equals(other);
    }

    /// <summary>
    /// Hash code based on the Whisper model name.
    /// </summary>
    /// <returns>The hash code for this model metadata.</returns>
    public override int GetHashCode()
    {
        return ModelName.GetHashCode();
    }

    /// <summary>
    /// Determines whether two model metadata instances are equal.
    /// </summary>
    /// <param name="left">Left model metadata operand.</param>
    /// <param name="right">Right model metadata operand.</param>
    /// <returns><see langword="true"/> when both operands are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(WhisperModel? left, WhisperModel? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two model metadata instances are different.
    /// </summary>
    /// <param name="left">Left model metadata operand.</param>
    /// <param name="right">Right model metadata operand.</param>
    /// <returns><see langword="true"/> when both operands are different; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(WhisperModel? left, WhisperModel? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Whisper model name used by the CLI.
    /// </summary>
    public required WhisperModelName ModelName          { get; init; }

    /// <summary>
    /// Model parameter count in millions.
    /// </summary>
    public required int              Parameters         { get; init; } 

    /// <summary>
    /// English-only model name, when available.
    /// </summary>
    public          string?          English_only_model { get; init; }

    /// <summary>
    /// Multilingual model name.
    /// </summary>
    public required string           Multilingual_model { get; init; }

    /// <summary>
    /// Approximate required VRAM in gigabytes.
    /// </summary>
    public required int              Required_VRAM_GB   { get; init; }

    /// <summary>
    /// Relative inference speed multiplier.
    /// </summary>
    public required int              Relative_speed     { get; init; } // mnożnik (np. 10 = ~10x)

    /// <summary>
    /// All predefined Whisper model metadata entries.
    /// </summary>
    public static IEnumerable<WhisperModel> All => new[]
    {
        Tiny,
        Base,
        Small,
        Medium,
        Large,
        Turbo
    };

    /// <summary>
    /// Metadata for the tiny Whisper model.
    /// </summary>
    public static WhisperModel Tiny => new()
    {
        ModelName          = new WhisperModelName("tiny"),
        Parameters         = 39,
        English_only_model = "tiny.en",
        Multilingual_model = "tiny",
        Required_VRAM_GB   = 1,
        Relative_speed     = 10
    };

    /// <summary>
    /// Metadata for the base Whisper model.
    /// </summary>
    public static WhisperModel Base => new()
    {
        ModelName          = new WhisperModelName("base"),
        Parameters         = 74,
        English_only_model = "base.en",
        Multilingual_model = "base",
        Required_VRAM_GB   = 1,
        Relative_speed     = 7
    };

    /// <summary>
    /// Metadata for the small Whisper model.
    /// </summary>
    public static WhisperModel Small => new()
    {
        ModelName          = new WhisperModelName("small"),
        Parameters         = 244,
        English_only_model = "small.en",
        Multilingual_model = "small",
        Required_VRAM_GB   = 2,
        Relative_speed     = 4
    };

    /// <summary>
    /// Metadata for the medium Whisper model.
    /// </summary>
    public static WhisperModel Medium => new()
    {
        ModelName          = new WhisperModelName("medium"),
        Parameters         = 769,
        English_only_model = "medium.en",
        Multilingual_model = "medium",
        Required_VRAM_GB   = 5,
        Relative_speed     = 2
    };

    /// <summary>
    /// Metadata for the large Whisper model.
    /// </summary>
    public static WhisperModel Large => new()
    {
        ModelName          = new WhisperModelName("large"),
        Parameters         = 1550,
        English_only_model = null,
        Multilingual_model = "large",
        Required_VRAM_GB   = 10,
        Relative_speed     = 1
    };

    /// <summary>
    /// Metadata for the turbo Whisper model.
    /// </summary>
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
