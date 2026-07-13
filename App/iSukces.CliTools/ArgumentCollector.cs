using System.Globalization;

namespace iSukces.CliTools;

public class ArgumentCollector
{
    public void Add(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        list.Add(text);
    }

    public void AddOption(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        list.Add(OptionPrefix+text);
    }

    public void Add(bool? value, string ifTrue, string ifFalse)
    {
        if (value is null) return;
        var v = value.Value ? ifTrue : ifFalse;
        if (string.IsNullOrEmpty(v)) return;
        list.Add(OptionPrefix + v);
    }

    public void Add(int? number, string key)
    {
        if (number is null)
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(number.Value.ToString(CultureInfo.InvariantCulture));
    }

    public void Add(double? number, string key)
    {
        if (number is null)
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(number.Value.ToString(CultureInfo.InvariantCulture));
    }

    public void Add(string? text, string key)
    {
        if (string.IsNullOrEmpty(text))
            return;
        list.Add($"{OptionPrefix}{key}");
        list.Add(text);
    }


    public void AddSnake(object? value, string name)
    {
        var txt = value?.ToString()?.ToSnake();
        Add(txt, name);
    }

    public string[] ToArray()
    {
        return list.ToArray();
    }

    public string OptionPrefix { get; set; } = "--";

    private readonly List<string> list = new List<string>();
}
