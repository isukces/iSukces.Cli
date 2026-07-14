using System.Text.RegularExpressions;

namespace iSukces.Cli.Python;

/// <summary>
/// Base type for strongly typed textual version values.
/// </summary>
public abstract class VersionBase
{
    /// <summary>
    /// Creates a strongly typed textual version value.
    /// </summary>
    /// <param name="value">Textual version value.</param>
    protected VersionBase(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        value = value.Trim();
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be empty or whitespace.", nameof(value));
        Value = value;
    }
    
    /// <summary>
    /// Parsed version value from text using the specified regular expression.
    /// </summary>
    /// <param name="text">Text that may contain a version value.</param>
    /// <param name="filter">Regular expression used to extract the version value.</param>
    /// <returns>Parsed version value, or null when the text does not match the filter.</returns>
    public static string? TryParse(string? text, Regex filter)
    {
        text = text?.Trim();
        if (string.IsNullOrEmpty(text))
            return null;
        var match = filter.Match(text);
        if (!match.Success)
            return null;
        return match.Groups[1].Value;
    }
    /// <summary>
    /// Text representation of the version value.
    /// </summary>
    /// <returns>Version value.</returns>
    public override string ToString()
    {
        return Value;
    }
    /// <summary>
    /// Version value.
    /// </summary>
    public string Value { get; }
}
