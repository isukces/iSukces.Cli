using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

/// <summary>
/// Python interpreter version entry reported by the Windows Python launcher.
/// </summary>
public sealed class PythonVersionInfo
{
    /// <summary>
    /// Full Python interpreter version.
    /// </summary>
    public Version Version { get; init; } = default!;

    /// <summary>
    /// Interpreter architecture in bits when reported by the launcher.
    /// </summary>
    public int? Architecture { get; init; }

    /// <summary>
    /// Default interpreter flag reported by the launcher.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Raw launcher output line used to create the entry.
    /// </summary>
    public string Raw { get; init; } = default!;

    /// <summary>
    /// Text representation of the Python version.
    /// </summary>
    /// <returns>Python version string.</returns>
    public override string ToString() => Version.ToString();

    /// <summary>
    /// Compatibility check against a major and minor Python version string.
    /// </summary>
    /// <param name="version">Major and minor Python version string to compare with.</param>
    /// <returns>True when the interpreter has the requested major and minor version; otherwise, false.</returns>
    public bool Compatible(string version)
    {
        var v = Version.ToString().Split('.');
        var ve = string.Join(".", v.Take(2));
        return ve == version;
    }
}



/// <summary>
/// Parser for Windows Python launcher version listings.
/// </summary>
public static class PyLauncherParser
{
    private static readonly Regex LineRegex = new(
        @"^\s*-V:(?<major>\d+)\.(?<minor>\d+)(?:\[-(?<arch>\d+)\])?\s*(?<default>\*)?\s+Python\s+(?<full>\d+\.\d+(?:\.\d+)?)(?:\s+\((?<arch2>\d+)-bit\))?",
        RegexOptions.Compiled);
    /// <summary>
    /// Parsed Python version entry from a launcher output line.
    /// </summary>
    /// <param name="line">Windows Python launcher output line.</param>
    /// <returns>Parsed version entry, or null when the line does not describe a Python version.</returns>
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
    /// <summary>
    /// Parsed Python version entries from launcher output lines.
    /// </summary>
    /// <param name="lines">Windows Python launcher output lines.</param>
    /// <returns>Parsed Python version entries.</returns>
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
