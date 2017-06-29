using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceTreeItem
    {
        public ResourceTreeItem(long id, long? parentId, string keyFragment, string resourceKey, bool isLeaf, ICollection<ResourceItem> translations, bool allowDelete, bool isHidden, string path)
        {
            Id = id;
            ParentId = parentId;
            KeyFragment = keyFragment ?? throw new ArgumentNullException(nameof(keyFragment));
            ResourceKey = resourceKey;
            IsLeaf = isLeaf;
            Translations = translations;
            AllowDelete = allowDelete;
            IsHidden = isHidden;
            Path = path;
        }

        public long Id { get; }

        public long? ParentId { get; }

        public string ResourceKey { get; }

        public bool IsLeaf { get; }

        public ICollection<ResourceItem> Translations { get; }

        public bool AllowDelete { get; }

        public bool IsHidden { get; set; }

        public string Path { get; }

        public string KeyFragment { get; }
    }
}
