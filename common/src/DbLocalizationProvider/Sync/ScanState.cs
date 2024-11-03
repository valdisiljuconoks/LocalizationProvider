using System.Collections.Concurrent;

namespace DbLocalizationProvider.Sync;

/// <summary>
/// Holds a state of scanning process.
/// </summary>
public class ScanState
{
    /// <summary>
    /// Cache of [UseResource] usages (this is for perf reasons).
    /// </summary>
    public ConcurrentDictionary<string, string> UseResourceAttributeCache { get; } =
        new();
}
