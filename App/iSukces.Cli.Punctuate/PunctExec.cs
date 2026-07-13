using System.Diagnostics;
using System.Text;
using iSukces.CliTools;
using iSukces.Cli.Python;
using Newtonsoft.Json;

namespace iSukces.Cli.Punctuate;

public sealed class PunctExec(PythonVenv venv)
{
    private static string LoadFromResource(string fragment)
    {
        var assembly = typeof(PunctExec).Assembly;
        var names    = assembly.GetManifestResourceNames();

        var streamName = names.First(n => n.Contains(fragment, StringComparison.InvariantCultureIgnoreCase));

        using var s      = assembly.GetManifestResourceStream(streamName);
        using var reader = new StreamReader(s, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var       text   = reader.ReadToEnd();
        return text;
    }


    private static string RemovePunctuationAndNormalize(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Pojedyncze przejście in-place: znaki interpunkcyjne i białe znaki
        // są zamieniane na spację, a ciągi kolejnych spacji są zwijane do
        // jednej. Dwie alokacje (ToCharArray + new string) zamiast 6+ w
        // oryginalnej implementacji z wieloma Replace i pętlą while.
        //
        // Wyjątek: kropka między cyframi (np. 234.33) jest zachowywana,
        // ponieważ jest częścią liczby dziesiętnej, a nie interpunkcją.
        var chars        = text.ToCharArray();
        var write        = 0;
        var lastWasSpace = false;

        for (var i = 0; i < chars.Length; i++)
        {
            var c = chars[i];

            // Kropka między cyframi jest częścią liczby — zachowujemy ją.
            if (c == '.'
                && write > 0 && char.IsDigit(chars[write - 1])
                && i + 1 < chars.Length && char.IsDigit(chars[i + 1]))
            {
                chars[write++] = c;
                lastWasSpace   = false;
                continue;
            }

            var isPunctuation = IsAnyPunctuation(c);

            if (isPunctuation)
            {
                if (!lastWasSpace)
                {
                    chars[write++] = ' ';
                    lastWasSpace   = true;
                }
            }
            else
            {
                chars[write++] = c;
                lastWasSpace   = false;
            }
        }

        return new string(chars, 0, write);
    }
    
    private static bool IsAnyPunctuation(char c)
    {
        return c is ' ' or '.' or ',' or ';' or '?' or '-' or '!' or '\r' or '\n' or '|';
    }

    
    public async Task<List<string>> FromFile(string fileName)
    {
        var text = await File.ReadAllTextAsync(fileName).ConfigureAwait(false);
        return await Process(text).ConfigureAwait(false);
    }
 

    public async Task<List<string>> Process(string text)
    {
        var fileCache = StateHolder?.GetCacheFileName("punctuation-state.json");
        
        var state = await ProcessState.TryLoad(fileCache).ConfigureAwait(false);
        if (state is not null && string.IsNullOrEmpty(state.Text))
        {
            return state.Sentences;
        }
        
        
        text = RemovePunctuationAndNormalize(text);
        state ??= new ProcessState
        {
            Text = text
        };

        var python = LoadFromResource("punctuate_process.py");
        var uid    = $"job_{Guid.NewGuid():N}";
        try
        {
            await File.WriteAllTextAsync(Path.Combine(venv.WorkingDirectory, uid + ".py"), python).ConfigureAwait(false);
          

            while (true)
            {
                var currentText1 = state.Text;
                var currentText2 = "";
                if (currentText1.Length > 3500)
                {
                    currentText2 = currentText1[3000..];
                    currentText1 = currentText1[..3000];
                }

                var applyResult = await SingleIteration(currentText1, currentText2.Length>0).ConfigureAwait(false);
                state.Sentences.AddRange(applyResult.AcceptedSentences);
                var chars = currentText1.Length - applyResult.Left.Length;
                if (applyResult.AcceptedSentences.Count == 0)
                {
                    Debug.WriteLine("");
                }
                Console.WriteLine("Zaakceptowane " + Nouns.Sentences(applyResult.AcceptedSentences.Count) + ", " + Nouns.Chars(chars) + ", zostało " +
                                  Nouns.Chars(state.Text.Length));
                state.Text = applyResult.Left + currentText2;
                await ProcessState.TrySave(fileCache, state).ConfigureAwait(false);
                if (string.IsNullOrEmpty(state.Text))
                    break;
            }

            return state.Sentences;
        }
        finally
        {
            string[] files = ["py", "txt", "json"];
            foreach (var file in files)
            {
                var path = Path.Combine(venv.WorkingDirectory, $"{uid}.{file}");
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        async Task<PunctuationPostProcessor.Result> SingleIteration(string chunk, bool ignoreLastSentence)
        {
            var cacheFileName = ExternalConfigs.CacheFileNameProvider?.CreateCacheFileName(chunk, "punctuate");
            //Sha1Cache.FromText(chunk, "punctuate");
            var json =await  Cache.TryGetFromCache(chunk, cacheFileName);
            if (string.IsNullOrEmpty(json))
            {
                await File.WriteAllTextAsync(Path.Combine(venv.WorkingDirectory, uid + ".txt"), chunk).ConfigureAwait(false);

                var cli = new CliTools.Cli
                {
                    FileName         = "python.exe",
                    Arguments        = [uid + ".py", uid + ".txt", uid + ".json"],
                    ThrowIfFailed    = true,
                    OutputMode       = CliOutputMode.None,
                    Redirect         = RedirctToConsole.None
                }
                    .WithVenvSettingsAndWorkingDirectory(venv);

                var result   = await cli.RunAsync().ConfigureAwait(false);
                var jsonFile = Path.Combine(venv.WorkingDirectory, uid + ".json");
                json = await File.ReadAllTextAsync(jsonFile).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(cacheFileName)) 
                    await Cache.SaveToCache(chunk, json, cacheFileName);
            }

            var applyResult = PunctuationPostProcessor.Apply(chunk, json, true, ignoreLastSentence);
            return applyResult;

       
        }

      
    }
    
    sealed class Cache
    {
        public string Input  { get; set; }
        public string Output { get; set; }


        internal static async Task<string?> TryGetFromCache(string input, string? rawCache)
        {
            if (rawCache is null || !File.Exists(rawCache)) return null;
            var readAllText = await File.ReadAllTextAsync(rawCache).ConfigureAwait(false);
            var c           = JsonConvert.DeserializeObject<Cache>(readAllText);
            if (c?.Input == input)
                return c.Output;

            return null;
        }

        internal static async Task SaveToCache(string input, string output, string fileName)
        {
            var c = new Cache
            {
                Input  = input,
                Output = output
            };
            var json = JsonConvert.SerializeObject(c, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);
        }
    }
    public IStateFilesHolder? StateHolder { get; set; }

    private class ProcessState
    {
        public static async Task<ProcessState?> TryLoad(string? fileCache)
        {
            if (!File.Exists(fileCache))
                return null;
            var json  = await File.ReadAllTextAsync(fileCache).ConfigureAwait(false);
            var cache = JsonConvert.DeserializeObject<ProcessState>(json);
            return cache;
        }

        public static async Task TrySave(string? fileCache, ProcessState state)
        {
            if (string.IsNullOrEmpty(fileCache))
                return;
            var json = JsonConvert.SerializeObject(state, Formatting.Indented);
            await File.WriteAllTextAsync(fileCache, json).ConfigureAwait(false);
        }

        public List<string> Sentences { get; }      = [];
        public string       Text      { get; set; } = "";
    }
}
