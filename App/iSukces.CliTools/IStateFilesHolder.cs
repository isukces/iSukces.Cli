using Newtonsoft.Json;

namespace iSukces.CliTools;

/// <summary>
/// Storage abstraction for named cache files.
/// </summary>
public interface IStateFilesHolder
{
    /// <summary>
    /// Cache file name associated with a logical name.
    /// </summary>
    /// <param name="name">Logical cache file name.</param>
    /// <returns>The physical cache file name for the logical name.</returns>
    string GetCacheFileName(string name);
}

/// <summary>
/// Extension methods for cached state file access.
/// </summary>
public static class DocumentProcessingFolderExt
{
    extension(IStateFilesHolder cache)
    {
        /// <summary>
        /// Provides cached text content or creates it when the cache file is missing.
        /// </summary>
        /// <param name="key">Cache key used to derive the text file name.</param>
        /// <param name="func">Asynchronous factory that creates the text content when the cache is missing.</param>
        /// <returns>The cached or newly created text content.</returns>
        public async Task<string> GetOrCreate(string key, Func<Task<string>> func)
        {
            var fn = cache.GetCacheFileName(key + ".txt");
            if (File.Exists(fn))
                return await File.ReadAllTextAsync(fn);


            var txt = await func().ConfigureAwait(false);
            await File.WriteAllTextAsync(fn, txt).ConfigureAwait(false);
            return txt;
        }

        /// <summary>
        /// Provides a cached JSON value or creates it when the cache file is missing.
        /// </summary>
        /// <param name="key">Cache key used to derive the JSON file name.</param>
        /// <param name="func">Asynchronous factory that creates the value when the cache is missing.</param>
        /// <typeparam name="T">Type of the cached JSON value.</typeparam>
        /// <returns>The cached or newly created value.</returns>
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
