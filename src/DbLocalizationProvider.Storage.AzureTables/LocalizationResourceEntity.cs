using System;
using Microsoft.Azure.Cosmos.Table;

namespace DbLocalizationProvider.Storage.AzureTables
{
    public class LocalizationResourceEntity : TableEntity
    {
        public const string PartitionKey = "resources";

        public LocalizationResourceEntity() { }

        public LocalizationResourceEntity(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            base.PartitionKey = PartitionKey;
            RowKey = id;
        }

        public DateTime ModificationDate { get; set; }

        public string Author { get; set; }

        public bool FromCode { get; set; }

        public bool IsHidden { get; set; }

        public bool IsModified { get; set; }

        public string Notes { get; set; }

        public string Translations { get; set; }
    }
}
