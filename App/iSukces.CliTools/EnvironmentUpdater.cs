namespace iSukces.CliTools;

public class EnvironmentUpdater
{
    public static EnvironmentUpdater? operator +(EnvironmentUpdater? a, EnvironmentUpdater? b)
    {
        if (a is null) return b;
        if (b is null) return a;

        var result =  new EnvironmentUpdater();

        var addToPathAtBeginning =
            (a.AddToPathAtBeginning ?? [])
            .Concat(b.AddToPathAtBeginning ?? [])
            .Distinct();
        
        result.AddToPathAtBeginning.AddRange(addToPathAtBeginning);
        foreach (var (key, value) in a.SetVariables ?? [])
            result.SetVariables[key] = value;
        foreach (var (key, value) in b.SetVariables ?? [])
            result.SetVariables[key] = value;

        return result;
    }

    public List<string>                AddToPathAtBeginning { get; } = [];
    public Dictionary<string, string?> SetVariables         { get; } = new(StringComparer.OrdinalIgnoreCase);


    private static IEnumerable<string> Concat(
        IEnumerable<string?>? a,
        IEnumerable<string?>? b)
    {
        if (a is null) return Q(b);
        if (b is null) return Q(a);
        return Q(a.Concat(b));

        static IEnumerable<string> Q(IEnumerable<string?>? a)
        {
            if (a is null) return [];
            return a
                .Select(q => q?.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct()!;
        }
    }


    public IEnumerable<KeyValuePair<string, string>> GetEnvironmentVariables()
    {
        if (SetVariables is not null && SetVariables.Count > 0)
            foreach (var x in SetVariables)
                if (x.Value is not null)
                    yield return new KeyValuePair<string, string>(x.Key, x.Value);

        if (AddToPathAtBeginning is null || AddToPathAtBeginning.Count <= 0)
            yield break;
        var currentPath = Environment.GetEnvironmentVariable(PathKey);
        var newPathItems = Concat(
            AddToPathAtBeginning,
            currentPath?.Split(';')
        );
        var valueToSet = string.Join(";", newPathItems);
        yield return new KeyValuePair<string, string>(PathKey, valueToSet);
    }

    public void Setup()
    {
        foreach (var kv in GetEnvironmentVariables())
            Environment.SetEnvironmentVariable(kv.Key, kv.Value);

    }

    const string PathKey = "PATH";

    public EnvironmentUpdater? GetBackup()
    {
        var setvariables = new Dictionary<string, string?>();
        if (SetVariables is not null && SetVariables.Count > 0)
            foreach (var x in SetVariables)
            {
                setvariables[x.Key] = Environment.GetEnvironmentVariable(x.Key);
            }

        if (AddToPathAtBeginning is not null && AddToPathAtBeginning.Count > 0)
        {

            setvariables[PathKey] = Environment.GetEnvironmentVariable(PathKey);
        }

        if (setvariables.Count == 0)
            return null;
        var tmp = new EnvironmentUpdater();
        tmp.Append(setvariables);
        return tmp;
    }

    public IDisposable Activate()
    {
        var backup = GetBackup();
        Setup();

        return new ActionDisposable(() =>
        {
            backup?.Setup();
        });
    }

    public static EnvironmentUpdater Make(string key, string value)
    {
        var make = new EnvironmentUpdater
        {
            SetVariables =
            {
                [key] = value
            }
        };
        return make;
    }

    public void SetVariable(string key, string? value)
    {
        SetVariables[key] =   value;
    }

    public void Append(EnvironmentUpdater? updater)
    {
        Append(updater?.SetVariables);
        Append(updater?.AddToPathAtBeginning);
    }

    private void Append(List<string>? list)
    {
        if (list is null || list.Count == 0) return;
        foreach (var path in list)
            AddToPathAtBeginning.Add(path);
    }

    private void Append(Dictionary<string, string?>? dictionary)
    {
        if (dictionary is null || dictionary.Count == 0) return;
        foreach (var kv in dictionary)
            SetVariables[kv.Key] = kv.Value;
    }

    public bool SearchFile(string shortName, bool useEnvironmentPath, out string found)
    {
        var searchPaths = GetPaths().Distinct(StringComparer.OrdinalIgnoreCase);
        #if DEBUGx
        searchPaths = searchPaths.ToArray();
        #endif
        foreach (var i in searchPaths)
        {
            var fullName = Path.Combine(i, shortName);
            if (File.Exists(fullName))
            {
                found = fullName;
                return true;
            }
        }

        found = "";
        return false;

        IEnumerable<string> GetPaths()
        {
            foreach (var p1 in AddToPathAtBeginning)
            {
                yield return p1;
            }

            if (SetVariables.TryGetValue("PATH", out var list1))
            {
                if (!string.IsNullOrEmpty(list1))
                    foreach (var i in list1.Split(';'))
                    {
                        yield return i;
                    }
            }

            if (useEnvironmentPath)
            {
                var list2 = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(list2))
                {
                    foreach (var i in list2.Split(';'))
                    {
                        yield return i;
                    }
                }
            }
            
        }
    }
}
