using Newtonsoft.Json;

namespace iSukces.Cli.Punctuate;

internal sealed class TokenPredictionModel
{
    // {"entity": "0", "score": 0.9999549388885498, "index": 1, "word": "в–ЃDo", "start": 0, "end": 2}
    [JsonProperty("entity")]
    public string Entity { get; set; } = "0";

    [JsonProperty("score")]
    public double Score { get; set; } = 0;

    [JsonProperty("index")]
    public int Index { get; set; } = 0;

    [JsonProperty("word")]
    public string Word { get; set; }

    [JsonProperty("start")]
    public int Start { get; set; }

    [JsonProperty("end")]
    public int End { get; set; }
}
