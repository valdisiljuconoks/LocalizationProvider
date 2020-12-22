using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.EnumTests
{
    [LocalizedResource]
    public enum SampleEnumWithDisplayAttribute
    {
        None = 0,
        [Display(Name = "This is new")] New = 1,
        Open = 2
    }
}
