using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

public sealed class PythonVersionInfo
{
    public Version Version      { get; init; } = default!;
    public int?    Architecture { get; init; }   // 32 / 64 / null
    public bool    IsDefault    { get; init; }
    public string  Raw          { get; init; } = default!;
    
    public override string ToString() => Version.ToString();

    public bool Compatible(string version)
    {
        var v = Version.ToString().Split('.');
        var ve = string.Join(".", v.Take(2));
        return ve == version;
    }
}




public static class PyLauncherParser
{
    private static readonly Regex LineRegex = new(
        @"^\s*-V:(?<major>\d+)\.(?<minor>\d+)(?:\[-(?<arch>\d+)\])?\s*(?<default>\*)?\s+Python\s+(?<full>\d+\.\d+(?:\.\d+)?)(?:\s+\((?<arch2>\d+)-bit\))?",
        RegexOptions.Compiled);

    public static PythonVersionInfo? TryParse(string line)
    {
        var m = LineRegex.Match(line);
        if (!m.Success)
            return null;

        var fullVersion = Version.Parse(m.Groups["full"].Value);

        int? arch = null;

        if (m.Groups["arch"].Success)
            arch = int.Parse(m.Groups["arch"].Value);
        else if (m.Groups["arch2"].Success)
            arch = int.Parse(m.Groups["arch2"].Value);

        return new PythonVersionInfo
        {
            Version      = fullVersion,
            Architecture = arch,
            IsDefault    = m.Groups["default"].Success,
            Raw          = line
        };
    }

    public static IEnumerable<PythonVersionInfo> ParseMany(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var info = TryParse(line);
            if (info is not null)
                yield return info;
        }
    }
}
