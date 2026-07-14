// Option names, descriptions and examples are based on public OpenAI Whisper CLI documentation
// and `whisper --help` output; this file does not include OpenAI Whisper source code.

using iSukces.CliTools;
using iSukces.Cli.Python;

namespace iSukces.Cli.Whisper;

/// <summary>
/// Configuration and execution wrapper for OpenAI Whisper CLI calls.
/// </summary>
public sealed class WhisperCli
{
    private sealed class WhisperArgumentCollector : ArgumentCollector
    {
        /// <summary>
        /// Adds a Boolean Whisper option by mapping the value to custom true or false text.
        /// </summary>
        /// <param name="value">Boolean value to map, or <see langword="null"/> to skip the option.</param>
        /// <param name="name">Option name without the configured prefix.</param>
        /// <param name="trueText">Option value used when <paramref name="value"/> is <see langword="true"/>.</param>
        /// <param name="falseText">Option value used when <paramref name="value"/> is <see langword="false"/>.</param>
        public void Add(bool? value, string name, string trueText, string falseText)
        {
            if (value is null)
                return;
            Add(value.Value ? trueText : falseText, name);
        }
    }

    /// <summary>
    /// Command-line arguments created from the current Whisper configuration.
    /// </summary>
    /// <returns>The command-line arguments for the Whisper CLI process.</returns>
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

    /// <summary>
    /// Input audio or video file processed by Whisper.
    /// </summary>
    public required string InputFile { get; set; }

    /// <summary>
    /// Python virtual environment used to run Whisper.
    /// </summary>
    public required PythonVenv Venv { get; set; }

    /// <summary>
    /// Runs the configured Whisper CLI process asynchronously.
    /// </summary>
    /// <param name="workingDirectory">Working directory for the Whisper process.</param>
    /// <returns>The captured CLI execution result.</returns>
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

    /// <summary>
    /// Whisper model name used for inference.
    /// </summary>
    public WhisperModelName     Model        { get; set; }

    /// <summary>
    /// Source language code or name passed to Whisper.
    /// </summary>
    public string?              Language     { get; set; }

    /// <summary>
    /// Output directory for generated transcription files.
    /// </summary>
    public string?              OutputDir    { get; set; }

    /// <summary>
    /// Output file format requested from Whisper.
    /// </summary>
    public WhisperOutputFormat? OutputFormat { get; set; }

    /// <summary>
    /// Whisper task mode.
    /// </summary>
    public WhisperTask          Task         { get; set; } = WhisperTask.Transcribe;

    /// <summary>
    /// Sampling temperature used by Whisper decoding.
    /// </summary>
    public double?              Temperature  { get; set; }


    /// <summary>
    /// Optional text provided as the first-window prompt.
    /// </summary>
    public string? InitialPrompt { get; set; }

    /// <summary>
    /// FP16 inference preference.
    /// </summary>
    public bool? Fp16 { get; set; }


    /// <summary>
    /// Temperature increment used for fallback decoding attempts.
    /// </summary>
    public double? TemperatureIncrementOnFallback { get; set; }

    /// <summary>
    /// Start time of the processed audio range.
    /// </summary>
    public TimeSpan? Begin { get; set; }

    /// <summary>
    /// End time of the processed audio range.
    /// </summary>
    public TimeSpan? End   { get; set; }
    
    
    /// <summary>
    /// Previous-text conditioning preference passed to Whisper.
    /// </summary>
    public bool? ConditionOnPreviousText { get; set; }

    /// <summary>
    /// Initial prompt carry-over preference passed to Whisper.
    /// </summary>
    public bool? CarryInitialPrompt    { get; set; }
}

/// <summary>
/// Whisper processing task mode.
/// </summary>
public enum WhisperTask
{
    /// <summary>
    /// Transcription task that preserves the source language.
    /// </summary>
    Transcribe,

    /// <summary>
    /// Translation task that converts speech to English text.
    /// </summary>
    Translate
}

/// <summary>
/// Output file format produced by Whisper.
/// </summary>
public enum WhisperOutputFormat
{
    /// <summary>
    /// Plain text output format.
    /// </summary>
    Txt,

    /// <summary>
    /// WebVTT subtitle output format.
    /// </summary>
    Vtt,

    /// <summary>
    /// SubRip subtitle output format.
    /// </summary>
    Srt,

    /// <summary>
    /// Tab-separated values output format.
    /// </summary>
    Tsv,

    /// <summary>
    /// JSON output format.
    /// </summary>
    Json,

    /// <summary>
    /// All supported output formats.
    /// </summary>
    All
}
