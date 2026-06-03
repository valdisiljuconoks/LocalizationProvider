using System;

namespace DbLocalizationProvider.Storage.AzureTables;

public class LocalizationResourceTranslationEntity
{
    public string Translation { get; set; }

    public string? Language { get; set; }

    public DateTime ModificationDate { get; set; }
}
