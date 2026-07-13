using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

public abstract class VersionBase
{
    protected VersionBase(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be empty or whitespace.", nameof(value));
        Value = value;
    }
    
    public static string? TryParse(string? x, Regex Filter)
    {
        x = x?.Trim();
        if (string.IsNullOrEmpty(x))
            return null;
        var match = Filter.Match(x);
        if (!match.Success)
            return null;
        return match.Groups[1].Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public string Value { get; }
}
