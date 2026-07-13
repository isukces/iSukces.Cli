// Option names, descriptions and examples are based on public OpenAI Whisper CLI documentation
// and `whisper --help` output; this file does not include OpenAI Whisper source code.

using iSukces.CliTools;
using iSukces.Cli.Python;

namespace iSukces.Cli.Whisper;

public sealed class WhisperCli
{
    private sealed class WhisperArgumentCollector : ArgumentCollector
    {
        public void Add(bool? value, string name, string trueText, string falseText)
        {
            if (value is null)
                return;
            Add(value.Value ? trueText : falseText, name);
        }
    }

    public string[] GetArguments()
    {
        // whisper audio.flac audio.mp3 audio.wav --model turbo
        // whisper japanese.wav --model medium --language Japanese --task translate
        var args = new WhisperArgumentCollector();
        args.Add(InputFile);
        args.Add(Model.Value, "model");
        args.Add(Language, "language");

        if (Task == WhisperTask.Translate)
            args.Add("translate", "task");

        args.AddSnake(OutputFormat, "output_format");
        args.Add(OutputDir, "output_dir");
        args.Add(Temperature, "temperature");
        args.Add(TemperatureIncrementOnFallback, "temperature_increment_on_fallback");
        
        args.Add(ConditionOnPreviousText, "condition_on_previous_text", "True", "False");
        args.Add(CarryInitialPrompt, "carry_initial_prompt", "True", "False");
        
        args.Add(InitialPrompt, "initial_prompt");
        if (Fp16 == false)
            args.Add("False", "fp16");

        var ts = Ts();
        if (!string.IsNullOrEmpty(ts))
            args.Add(ts, "clip_timestamps");

        return args.ToArray();

        string Ts()
        {
            
            if (End.HasValue)
            {
                var begin = Begin?.TotalSeconds ?? 0;
                return begin.ToInvariantString() +","+ End.Value.TotalSeconds.ToInvariantString();
            }

            if (Begin is null)
                return "";
            return Begin.Value.TotalSeconds.ToInvariantString();
        }
    }

    public required string InputFile { get; set; }

    public required PythonVenv Venv { get; set; }
    public async Task<CliResult> RunAsync(string? workingDirectory)
    {
      
        Console.WriteLine("Running whisper");
        var runner = new CliTools.Cli
            {
                FileName         = "whisper.exe",
                Arguments        = GetArguments().ToArray(),
                WorkingDirectory = workingDirectory,
                ThrowIfFailed    = true,
                OutputMode       = CliOutputMode.Tail,
            }
            .WithVenvSettings(Venv)
            .ExpandFileName();
        var r = await runner.RunAsync();
        Console.WriteLine("Done " + r.Duration.TotalMinutes + " minutes");
        return r;
    }

    public WhisperModelName     Model        { get; set; }
    public string?              Language     { get; set; }
    public string?              OutputDir    { get; set; }
    public WhisperOutputFormat? OutputFormat { get; set; }
    public WhisperTask          Task         { get; set; } = WhisperTask.Transcribe;
    public double?              Temperature  { get; set; }


    /// <summary>
    /// optional text to provide as a prompt for the first window. (default: None)
    /// </summary>
    public string? InitialPrompt { get; set; }

    /// <summary>
    /// whether to perform inference in fp16; True by default (default: True)
    /// </summary>
    public bool? Fp16 { get; set; }


    /// <summary>
    /// temperature to increase when falling back when the decoding fails to meet either of the thresholds below (default: 0.2)
    /// </summary>
    public double? TemperatureIncrementOnFallback { get; set; }

    public TimeSpan? Begin { get; set; }
    public TimeSpan? End   { get; set; }
    
    
    public bool? ConditionOnPreviousText { get; set; }
    public bool? CarryInitialPrompt    { get; set; }
}

public enum WhisperTask
{
    Transcribe,
    Translate
}

public enum WhisperOutputFormat
{
    Txt, Vtt, Srt, Tsv, Json, All
}
