using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI.ApiModels
{
    public class ResourceListItemApiModel
    {
        public ResourceListItemApiModel(string key, ICollection<ResourceItemApiModel> translations, bool syncedFromCode)
        {
            Key = key;
            Value = translations;
            SyncedFromCode = syncedFromCode;
            AllowDelete = !syncedFromCode;
        }

        public string Key { get; private set; }

        public ICollection<ResourceItemApiModel> Value { get; private set; }

        public bool SyncedFromCode { get; private set; }

        public bool AllowDelete { get; set; }

        public string DisplayKey { get; set; }
    }
}
