using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

using Self = PyTorchVersion;

/// <summary>
/// PyTorch package version.
/// </summary>
public sealed class PyTorchVersion : VersionBase, IEquatable<Self>
{
    /// <summary>
    /// Creates a PyTorch package version.
    /// </summary>
    /// <param name="value">PyTorch version value.</param>
    public PyTorchVersion(string value)
        : base(value)
    {
    }
    /// <summary>
    /// Equality comparison between two PyTorch versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when both versions are equal; otherwise, false.</returns>
    public static bool operator ==(Self? left, Self? right)
    {
        return Equals(left, right);
    }
    /// <summary>
    /// Inequality comparison between two PyTorch versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when the versions are not equal; otherwise, false.</returns>
    public static bool operator !=(Self? left, Self? right)
    {
        return !Equals(left, right);
    }
    /// <summary>
    /// Parsed PyTorch version from command output.
    /// </summary>
    /// <param name="x">Command output text that may contain a PyTorch version.</param>
    /// <returns>Parsed PyTorch version, or null when the text does not contain a supported version.</returns>
    public static Self? TryParse(string? x)
    {
        x = TryParse(x, Regex);
        if (string.IsNullOrEmpty(x))
            return null;
        return new Self(x);
    }
    /// <summary>
    /// Equality comparison with another PyTorch version.
    /// </summary>
    /// <param name="other">Other PyTorch version.</param>
    /// <returns>True when both versions are equal; otherwise, false.</returns>
    public bool Equals(Self? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }
    /// <summary>
    /// Equality comparison with another object.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns>True when the object is an equal PyTorch version; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Self other && Equals(other);
    }
    /// <summary>
    /// Hash code based on the PyTorch version value.
    /// </summary>
    /// <returns>Hash code for the version value.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static readonly Regex Regex = new Regex(@"^(?<version>\d+\.\d+\.\d+(?:\+\w+)?)\s*$");
}
