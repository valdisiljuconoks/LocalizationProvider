using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.EnumTests;

[LocalizedResource]
public enum SampleEnumWithAdditionalTranslations
{
    None = 0,

    [Display(Name = "This is new")]
    New = 1,

    [TranslationForCulture("Åpen", "no")]
    Open = 2
}
