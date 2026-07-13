using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

using Self = CudaToolkitVersion;

public class CudaToolkitVersion : VersionBase, IEquatable<Self>
{
    public CudaToolkitVersion(string value, DirectoryInfo toolkitDirectory)
        : base(value)
    {
        ToolkitDirectory = toolkitDirectory;
    }

    public static bool operator ==(Self? left, Self? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Self? left, Self? right)
    {
        return !Equals(left, right);
    }

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


    public bool Equals(Self? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Self)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public DirectoryInfo ToolkitDirectory { get; }

    private const string Filter = @"^v(\d+\.\d+)$";
    private static readonly Regex VersionFilterRegex = new Regex(Filter, RegexOptions.Compiled);
}
