using System.Globalization;
using System.Text.RegularExpressions;

namespace iSukces.Cli.Punctuate;

public sealed class Capitalizer
{
    private Capitalizer()
    {
    }

    /// <summary>
    ///     Buduje fragment wyrażenia regularnego (negative lookbehind),
    ///     który wyklucza kropkę następującą po znanym skrócie.
    ///     Wyrażenie jest case-insensitive (skróty angielskie występują
    ///     często z wielkiej litery, np. „Mr.”).
    /// </summary>
    private static string BuildPattern()
    {
        var items = Instance.Abbreviations
            .Select(a => @"\b" + Regex.Escape(a) + @"\b")
            .ToArray();

        var alternation = string.Join("|", items);
        return $@"(?<!(?i:{alternation}))";
    }

    public static string CapitalizeSentences(string text, bool addNewlineAfterSentence)
    {
        var culture = new CultureInfo("pl-PL");

        var newText = CapitalizeSentencesRegex.Replace(
            text,
            m =>
            {
                var prefix = m.Groups[1].Value;
                var letter = m.Groups[2].Value.ToUpper(culture);

                // Grupa 1 zawiera znak końca zdania i następujące po nim białe znaki.
                // Dla początku tekstu grupa jest pusta (zero-width ^), więc nic
                // nie zastępujemy. Gdy włączono opcję, białe znaki po znaku końca
                // zdania zastępujemy znakiem nowej linii, dzięki czemu każde
                // zdanie rozpoczyna się w nowym wierszu.
                if (addNewlineAfterSentence && prefix.Length > 0)
                    prefix += "\n";

                return prefix + letter;
            });
        return newText;
    }

    public IReadOnlyList<string> Abbreviations { get; set; } = ["etc"];

    public static Capitalizer Instance => CapitalizerHolder.SingleIstance;

    /// <summary>
    ///     Wyrażenie regularne wykrywające początek zdania po znaku końca zdania
    ///     (. ! ?). Fragment negative lookbehind wyklucza kropkę następującą po
    ///     znanych skrótach (np. „tzn.”, „tj.”, „np.”, „Mr.”), dzięki czemu
    ///     nie są one traktowane jako koniec zdania.
    ///     Grupa 1 przechwytuje znak końca zdania wraz z następującymi białymi
    ///     znakami (lub początek tekstu), grupa 2 — pierwszą literę nowego zdania.
    ///     Klasa znaków grupy 2 obejmuje zarówno małe, jak i wielkie litery
    ///     (polskie i łacińskie), ponieważ tekst wejściowy może już zawierać
    ///     wielkie litery po kropce — wtedy nadal chcemy wstawić nową linię.
    ///     Wywołanie ToUpper na już wielkiej literze jest operacją pustą (no-op).
    /// </summary>
    private static readonly Regex CapitalizeSentencesRegex =
        new(
            @"(^|" + BuildPattern() + @"[.!?]\s+)([a-ząćęłńóśźż])",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static class CapitalizerHolder
    {
        public static readonly Capitalizer SingleIstance = new Capitalizer();
    }
}
