namespace iSukces.CliTools;

/// <summary>
/// Text-related extension methods.
/// </summary>
public static class TextExtensions
{
    /// <summary>
    /// Converts text from PascalCase or camelCase to snake_case.
    /// </summary>
    /// <param name="x">Text to convert.</param>
    /// <returns>The converted snake_case text.</returns>
    public static string ToSnake(this string x)
    {
        if (string.IsNullOrEmpty(x))
            return x ?? string.Empty;

        var sb = new System.Text.StringBuilder(x.Length + 8);
        for (var i = 0; i < x.Length; i++)
        {
            var c = x[i];
            if (i == 0)
                c = char.ToLowerInvariant(c);
            var isUpper = char.IsUpper(c);

            if (i > 0 && isUpper)
            {
                var prev        = x[i - 1];
                var prevIsUpper = char.IsUpper(prev);
                var prevIsLower = char.IsLower(prev);
                var nextIsLower = i + 1 < x.Length && char.IsLower(x[i + 1]);

                if (prevIsLower || (prevIsUpper && nextIsLower))
                    sb.Append('_');
            }

            sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Non-empty text value or the provided default value.
    /// </summary>
    /// <param name="text">Primary text value.</param>
    /// <param name="defaultValue">Fallback text value.</param>
    /// <returns><paramref name="text"/> when it is not empty; otherwise, <paramref name="defaultValue"/> or an empty string.</returns>
    public static string Coalesce(this string? text, string? defaultValue)
    {
        if (string.IsNullOrEmpty(text))
            return defaultValue ?? string.Empty;
        return text;
    }
}
