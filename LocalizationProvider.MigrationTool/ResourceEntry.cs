using System.Collections.Generic;
using System.Diagnostics;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    [DebuggerDisplay("Key = {Key}, Translations = {Translations.Count}")]
    public class ResourceEntry
    {
        public ResourceEntry(string key)
        {
            Key = key;
            Translations = new List<ResourceTranslation>();
        }

        public string Key { get; }

        public ICollection<ResourceTranslation> Translations { get; }
    }
}
