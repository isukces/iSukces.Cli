using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace iSukces.Cli.Punctuate;

/// <summary>
/// Flagi sterujące zachowaniem metod <see cref="PunctuationPostProcessor.Apply"/>
/// i <see cref="PunctuationPostProcessor.ApplyFromFiles"/>.
/// </summary>
 
public static class PunctuationPostProcessor
{
    
    private static readonly Regex OldPunctuationRegex =
        new(@"(?<!\d)[\.,;:!\?…]+(?!\d)", RegexOptions.Compiled);

    private static readonly Regex WhiteSpaceRegex =
        new(@"\s+", RegexOptions.Compiled);

    private static readonly Regex WordRegex =
        new(@"\S+", RegexOptions.Compiled);

    private static readonly HashSet<string> AllowedPunctuation = new()
    {
        ".", ",", "?", "-", ":"
    };

    public static string PrepareForModel(string text)
    {
        text = OldPunctuationRegex.Replace(text, "");
        text = WhiteSpaceRegex.Replace(text, " ");
        return text.Trim();
    }

    public class Result
    {
        public required List<string>   AcceptedSentences { get; init; }
        public          required string Left              { get; init; }
    }

    public static Result ApplyFromFiles(string modelInputFile,
        string jsonFile,
        bool capitalize)
    {
        var modelInput = File.ReadAllText(modelInputFile, Encoding.UTF8);
        var json       = File.ReadAllText(jsonFile, Encoding.UTF8);

        return Apply(modelInput, json, capitalize, false);
    }

    public static Result Apply(string intputText, string json, bool capitalize, bool ignoreLastSentence)
    {
        var text = intputText;
        var lines2      = new List<string>();
        while (true)
        {
            var output = ApplyInternal(text);

            var sentences = output.Split('\n')
                .Select(a => a.Trim())
                .Where(a => a.Length > 0)
                .ToArray();
            const int sentenceMaxLength         = 800;
            var       acceptedCount = 0;

            var takeCount = sentences.Length;
            if (ignoreLastSentence)
                takeCount--;
            if (takeCount == 0)
                throw new Exception("No sentences to process");
            
            for (var index = 0; index < takeCount; index++)
            {
                var i = sentences[index];
                if (i.Length >= sentenceMaxLength)
                    break;
                lines2.Add(i);
                acceptedCount++;
            }

            if (acceptedCount == 0)
            {
                var l = sentences[0];
                var message = $"Too long sentence: {l}";
                throw new Exception(message);
            }

            var left = "";
            {
                var leftCount = sentences.Length - acceptedCount;
                switch (leftCount)
                {
                    case 1:
                        left = sentences[^1];
                        break;
                    case > 1:
                    {
                        var chunksLeft = sentences.Skip(acceptedCount).ToArray();
                        left = string.Join(" ", chunksLeft);
                        break;
                    }
                }
            }

            return new Result
            {
                AcceptedSentences = lines2,
                Left              = left
            };
        }

        string ApplyInternal(string textChunk)
        {
            var predictions = JsonConvert.DeserializeObject<List<TokenPredictionModel>>(json ?? "") ?? [];

            var words = WordRegex
                .Matches(textChunk)
                .Select(m => new WordSpan(
                    Text: m.Value,
                    Start: m.Index,
                    End: m.Index + m.Length))
                .ToList();

            var sb        = new StringBuilder();
            var predIndex = 0;

            for (var i = 0; i < words.Count; i++)
            {
                var word  = words[i];
                var label = "0";

                while (predIndex < predictions.Count &&
                       predictions[predIndex].End <= word.End)
                {
                    var pred = predictions[predIndex];

                    if (pred.End > word.Start)
                        label = NormalizeLabel(pred.Entity);

                    predIndex++;
                }

                sb.Append(word.Text);

                if (AllowedPunctuation.Contains(label))
                {
                    if (label == "-")
                        sb.Append(" -");
                    else
                        sb.Append(label);
                }

                if (i < words.Count - 1)
                    sb.Append(' ');
            }

            var result = sb.ToString().Trim();


            if (capitalize)
                result = Capitalizer.CapitalizeSentences(result, true);
            return result;
        }
    }

    private static string NormalizeLabel(string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return "0";

        return label.Trim();
    }

    // @"(^|" + SentenceEndAbbreviations.BuildLookbehindPattern() + @"[.!?]\s+)([a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ])",

    private sealed record WordSpan(
        string Text,
        int Start,
        int End
    );
}