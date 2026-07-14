namespace iSukces.CliTools;

/// <summary>
/// Global configuration hooks for external integrations.
/// </summary>
public static class ExternalConfigs
{
    /// <summary>
    /// Cache file name provider used by external components.
    /// </summary>
    public static ICacheFileNameProvider? CacheFileNameProvider { get; set; }
}
