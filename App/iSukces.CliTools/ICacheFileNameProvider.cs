namespace iSukces.CliTools;

/// <summary>
/// Provider of cache file names for text keys and subfolders.
/// </summary>
public interface ICacheFileNameProvider
{
    /// <summary>
    /// Creates a cache file name for the specified text key and subfolder.
    /// </summary>
    /// <param name="anyText">Text key used to derive the cache file name.</param>
    /// <param name="subFolder">Cache subfolder name.</param>
    /// <returns>The cache file name for the specified key and subfolder.</returns>
    string CreateCacheFileName(string anyText, string subFolder);
}
