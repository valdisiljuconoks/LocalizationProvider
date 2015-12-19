using System.Collections.Generic;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class LocalizableResourceEntry
    {
        public LocalizableResourceEntry(string key)
        {
            Key = key;
            Translations = new List<ResourceTranslation>();
        }

        public string Key { get; private set; }

        public ICollection<ResourceTranslation> Translations { get; private set; }
    }
}
