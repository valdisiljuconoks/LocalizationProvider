using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceListItem
    {
        public ResourceListItem(string key, ICollection<ResourceItem> translations, bool allowDelete, bool isHidden)
        {
            Key = key;
            Value = translations;
            AllowDelete = allowDelete;
            IsHidden = isHidden;
        }

        public string Key { get; }

        public ICollection<ResourceItem> Value { get; }

        public bool AllowDelete { get; }

        public string DisplayKey { get; set; }

        public bool IsHidden { get; }
    }
}
