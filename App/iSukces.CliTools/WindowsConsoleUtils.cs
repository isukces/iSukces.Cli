using System.Text;

namespace iSukces.CliTools;

public static class WindowsConsoleUtils
{
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
