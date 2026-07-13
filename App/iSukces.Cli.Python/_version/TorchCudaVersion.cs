using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

public sealed class TorchCudaVersion : VersionBase, IEquatable<TorchCudaVersion>
{
    public TorchCudaVersion(string value)
        : base(value)
    {
    }

    public static bool operator ==(TorchCudaVersion? left, TorchCudaVersion? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TorchCudaVersion? left, TorchCudaVersion? right)
    {
        return !Equals(left, right);
    }

    public static TorchCudaVersion? TryParse(string? x)
    {
        x = TryParse(x, Filter);
        if (string.IsNullOrEmpty(x))
            return null;
        return new TorchCudaVersion(x);
    }

    public bool Equals(TorchCudaVersion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is TorchCudaVersion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static readonly Regex Filter
        = new Regex(@"^(?<version>\d+\.\d+(?:\.\d+)?)\s*$");
}
