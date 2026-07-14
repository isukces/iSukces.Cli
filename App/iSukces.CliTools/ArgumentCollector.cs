using System.Globalization;

namespace iSukces.CliTools;

/// <summary>
/// Builder for command-line argument arrays.
/// </summary>
public class ArgumentCollector
{
    /// <summary>
    /// Adds a raw argument when the value is not empty.
    /// </summary>
    /// <param name="text">Argument text to add.</param>
    public void Add(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        list.Add(text);
    }

    /// <summary>
    /// Adds an option name prefixed with the configured option prefix.
    /// </summary>
    /// <param name="text">Option name without the configured prefix.</param>
    public void AddOption(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        list.Add(OptionPrefix+text);
    }

    /// <summary>
    /// Adds one of two option names according to a nullable Boolean value.
    /// </summary>
    /// <param name="value">Boolean value that selects the option name, or <see langword="null"/> to skip adding an option.</param>
    /// <param name="ifTrue">Option name used when <paramref name="value"/> is <see langword="true"/>.</param>
    /// <param name="ifFalse">Option name used when <paramref name="value"/> is <see langword="false"/>.</param>
    public void Add(bool? value, string ifTrue, string ifFalse)
    {
        if (value is null) return;
        var v = value.Value ? ifTrue : ifFalse;
        if (string.IsNullOrEmpty(v)) return;
        list.Add(OptionPrefix + v);
    }

    /// <summary>
    /// Adds an integer option with its value.
    /// </summary>
    /// <param name="number">Integer value to add, or <see langword="null"/> to skip the option.</param>
    /// <param name="key">Option name without the configured prefix.</param>
    public void Add(int? number, string key)
    {
        if (number is null)
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(number.Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Adds a floating-point option with its invariant-culture value.
    /// </summary>
    /// <param name="number">Floating-point value to add, or <see langword="null"/> to skip the option.</param>
    /// <param name="key">Option name without the configured prefix.</param>
    public void Add(double? number, string key)
    {
        if (number is null)
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(number.Value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Adds a text option with its value.
    /// </summary>
    /// <param name="text">Option value to add, or empty text to skip the option.</param>
    /// <param name="key">Option name without the configured prefix.</param>
    public void Add(string? text, string key)
    {
        if (string.IsNullOrEmpty(text))
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(text);
    }


    /// <summary>
    /// Adds an option after converting the value text to snake case.
    /// </summary>
    /// <param name="value">Value converted to text and then to snake case.</param>
    /// <param name="name">Option name without the configured prefix.</param>
    public void AddSnake(object? value, string name)
    {
        var txt = value?.ToString()?.ToSnake();
        Add(txt, name);
    }

    /// <summary>
    /// Complete argument array built by the collector.
    /// </summary>
    /// <returns>The collected command-line arguments.</returns>
    public string[] ToArray()
    {
        return list.ToArray();
    }

    /// <summary>
    /// Prefix used before option names.
    /// </summary>
    public string OptionPrefix { get; set; } = "--";

    private readonly List<string> list = new List<string>();
}
