using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

using Self = CudaToolkitVersion;

/// <summary>
/// CUDA Toolkit version with its installation directory.
/// </summary>
public class CudaToolkitVersion : VersionBase, IEquatable<Self>
{
    /// <summary>
    /// Creates a CUDA Toolkit version with its installation directory.
    /// </summary>
    /// <param name="value">CUDA Toolkit version value.</param>
    /// <param name="toolkitDirectory">CUDA Toolkit installation directory.</param>
    public CudaToolkitVersion(string value, DirectoryInfo toolkitDirectory)
        : base(value)
    {
        ToolkitDirectory = toolkitDirectory;
    }
    /// <summary>
    /// Equality comparison between two CUDA Toolkit versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when both versions are equal; otherwise, false.</returns>
    public static bool operator ==(Self? left, Self? right)
    {
        return Equals(left, right);
    }
    /// <summary>
    /// Inequality comparison between two CUDA Toolkit versions.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when the versions are not equal; otherwise, false.</returns>
    public static bool operator !=(Self? left, Self? right)
    {
        return !Equals(left, right);
    }
    /// <summary>
    /// Parsed CUDA Toolkit version from an installation directory.
    /// </summary>
    /// <param name="toolkitDirectory">Directory that may represent a CUDA Toolkit installation.</param>
    /// <returns>Parsed CUDA Toolkit version, or null when the directory is not a Toolkit version directory.</returns>
    public static Self? TryParse(DirectoryInfo? toolkitDirectory)
    {
        if (toolkitDirectory is null || !toolkitDirectory.Exists)
            return null;

        var x = toolkitDirectory.Name.Trim();
        if (string.IsNullOrEmpty(x))
            return null;
        var match = VersionFilterRegex.Match(x);
        if (!match.Success)
            return null;
        return new Self(match.Groups[1].Value, toolkitDirectory);
    }

    /// <summary>
    /// Equality comparison with another CUDA Toolkit version.
    /// </summary>
    /// <param name="other">Other CUDA Toolkit version.</param>
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
    /// <returns>True when the object is an equal CUDA Toolkit version; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Self)obj);
    }
    /// <summary>
    /// Hash code based on the CUDA Toolkit version value.
    /// </summary>
    /// <returns>Hash code for the version value.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    /// <summary>
    /// Text representation of the CUDA Toolkit version.
    /// </summary>
    /// <returns>CUDA Toolkit version value.</returns>
    public override string ToString()
    {
        return Value;
    }
    /// <summary>
    /// CUDA Toolkit installation directory.
    /// </summary>
    public DirectoryInfo ToolkitDirectory { get; }

    private const string Filter = @"^v(\d+\.\d+)$";
    private static readonly Regex VersionFilterRegex = new Regex(Filter, RegexOptions.Compiled);
}
