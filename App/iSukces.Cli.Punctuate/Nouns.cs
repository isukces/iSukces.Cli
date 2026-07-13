using iSukces.Translation;

namespace iSukces.Cli.Punctuate;

internal static class Nouns
{
    public static string Chars(int count)
    {
        var f = PolishGrammar.NounForm(count, "znak", "znaki", "znaków");
        return $"{count} {f}";
    }


    public static string Sentences(int count)
    {
        var f = PolishGrammar.NounForm(count, "zdanie", "zdania", "zdań");
        return $"{count} {f}";
    }

}
