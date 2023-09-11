using System;
using Azure;
using Azure.Data.Tables;

namespace DbLocalizationProvider.Storage.AzureTables
{
    public class LocalizationResourceEntity : ITableEntity
    {
        public const string PartitionKeyValue = "resources";

        public LocalizationResourceEntity() { }

        public LocalizationResourceEntity(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            RowKey = id;
            PartitionKey = PartitionKeyValue;
        }

        public DateTime ModificationDate { get; set; }
        public string Author { get; set; }
        public bool FromCode { get; set; }
        public bool IsHidden { get; set; }
        public bool IsModified { get; set; }
        public string Notes { get; set; }
        public string Translations { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
