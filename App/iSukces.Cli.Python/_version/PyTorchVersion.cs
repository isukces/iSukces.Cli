using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

using Self = PyTorchVersion;

public sealed class PyTorchVersion : VersionBase, IEquatable<Self>
{
    public PyTorchVersion(string value)
        : base(value)
    {
    }

    public static bool operator ==(Self? left, Self? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Self? left, Self? right)
    {
        return !Equals(left, right);
    }

    public static Self? TryParse(string? x)
    {
        x = TryParse(x, Regex);
        if (string.IsNullOrEmpty(x))
            return null;
        return new Self(x);
    }

    public bool Equals(Self? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Self other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static readonly Regex Regex = new Regex(@"^(?<version>\d+\.\d+\.\d+(?:\+\w+)?)\s*$");
}
