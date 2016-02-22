using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceListItem
    {
        public ResourceListItem(string key, ICollection<ResourceItem> translations, bool allowDelete)
        {
            Key = key;
            Value = translations;
            AllowDelete = allowDelete;
        }

        public string Key { get; private set; }

        public ICollection<ResourceItem> Value { get; private set; }

        public bool AllowDelete { get; private set; }
    }
}
