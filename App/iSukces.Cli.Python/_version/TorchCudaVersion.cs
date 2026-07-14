using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

/// <summary>
/// CUDA version reported by PyTorch.
/// </summary>
public sealed class TorchCudaVersion : VersionBase, IEquatable<TorchCudaVersion>
{
    /// <summary>
    /// Creates a CUDA version reported by PyTorch.
    /// </summary>
    /// <param name="value">CUDA version value reported by PyTorch.</param>
    public TorchCudaVersion(string value)
        : base(value)
    {
    }
    /// <summary>
    /// Equality comparison between two PyTorch CUDA versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when both versions are equal; otherwise, false.</returns>
    public static bool operator ==(TorchCudaVersion? left, TorchCudaVersion? right)
    {
        return Equals(left, right);
    }
    /// <summary>
    /// Inequality comparison between two PyTorch CUDA versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when the versions are not equal; otherwise, false.</returns>
    public static bool operator !=(TorchCudaVersion? left, TorchCudaVersion? right)
    {
        return !Equals(left, right);
    }
    /// <summary>
    /// Parsed PyTorch CUDA version from command output.
    /// </summary>
    /// <param name="x">Command output text that may contain a CUDA version.</param>
    /// <returns>Parsed PyTorch CUDA version, or null when the text does not contain a supported version.</returns>
    public static TorchCudaVersion? TryParse(string? x)
    {
        x = TryParse(x, Filter);
        if (string.IsNullOrEmpty(x))
            return null;
        return new TorchCudaVersion(x);
    }
    /// <summary>
    /// Equality comparison with another PyTorch CUDA version.
    /// </summary>
    /// <param name="other">Other PyTorch CUDA version.</param>
    /// <returns>True when both versions are equal; otherwise, false.</returns>
    public bool Equals(TorchCudaVersion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }
    /// <summary>
    /// Equality comparison with another object.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns>True when the object is an equal PyTorch CUDA version; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is TorchCudaVersion other && Equals(other);
    }
    /// <summary>
    /// Hash code based on the PyTorch CUDA version value.
    /// </summary>
    /// <returns>Hash code for the version value.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static readonly Regex Filter
        = new Regex(@"^(?<version>\d+\.\d+(?:\.\d+)?)\s*$");
}
