using Newtonsoft.Json;

namespace iSukces.CliTools;

public interface IStateFilesHolder
{
    string GetCacheFileName(string name);
}

public static class DocumentProcessingFolderExt
{
    extension(IStateFilesHolder cache)
    {
        public async Task<string> GetOrCreate(string key, Func<Task<string>> func)
        {
            var fn = cache.GetCacheFileName(key + ".txt");
            if (File.Exists(fn))
                return await File.ReadAllTextAsync(fn);


            var txt = await func().ConfigureAwait(false);
            await File.WriteAllTextAsync(fn, txt).ConfigureAwait(false);
            return txt;
        }

        public async Task<T> GetOrCreate<T>(string key, Func<Task<T>> func)
        {
            var fn = cache.GetCacheFileName(key + ".json");
            if (File.Exists(fn))
            {
                var json = await File.ReadAllTextAsync(fn);
                return JsonConvert.DeserializeObject<T>(json)!;
            }
            var value = await func().ConfigureAwait(false);
            {
                var json = JsonConvert.SerializeObject(value, Formatting.Indented);
                await File.WriteAllTextAsync(fn, json).ConfigureAwait(false);
            }
            return value;
        }
    }
}
