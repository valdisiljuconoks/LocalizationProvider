using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translation: {Translation}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(PropertyInfo info, string key, string translation)
        {
            Info = info;
            Key = key;
            Translation = translation;
        }

        public string Key { get; set; }

        public string Translation { get; set; }

        public PropertyInfo Info { get; set; }
    }
}
