using System.Text;

namespace iSukces.CliTools;

/// <summary>
/// Utilities for Windows console command-line handling.
/// </summary>
public static class WindowsConsoleUtils
{
    /// <summary>
    /// Escapes a command-line argument when Windows parsing rules require quoting.
    /// </summary>
    /// <param name="argument">Command-line argument to escape.</param>
    /// <returns>The original argument or its escaped representation.</returns>
    public static string EscapeCommandLineParameterIfNecessary(string argument)
    {
        if (string.IsNullOrEmpty(argument))
            return "\"\"";

        var needsQuotes = argument.Any(c => char.IsWhiteSpace(c) || c == '"');
        if (!needsQuotes)
            return argument;

        var sb = new StringBuilder();
        sb.Append('"');

        var backslashCount = 0;

        foreach (var c in argument)
        {
            switch (c)
            {
                case '\\':
                    backslashCount++;
                    continue;
                case '"':
                    sb.Append('\\', backslashCount * 2 + 1);
                    sb.Append('"');
                    backslashCount = 0;
                    continue;
            }

            sb.Append('\\', backslashCount);
            backslashCount = 0;
            sb.Append(c);
        }

        sb.Append('\\', backslashCount * 2);
        sb.Append('"');
        return sb.ToString();
    }
}
