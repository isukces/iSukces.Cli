namespace iSukces.CliTools;

public static class TextExtensions
{
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

    public static string Coalesce(this string? text, string? defaultValue)
    {
        if (string.IsNullOrEmpty(text))
            return defaultValue ?? string.Empty;
        return text;
    }
}
