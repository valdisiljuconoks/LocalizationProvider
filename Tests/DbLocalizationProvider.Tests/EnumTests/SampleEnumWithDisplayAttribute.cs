using System.ComponentModel.DataAnnotations;

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
