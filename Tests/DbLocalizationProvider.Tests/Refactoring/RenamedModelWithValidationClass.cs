using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelWithValidationClass")]
    public class RenamedModelWithValidationClass
    {
        [Required]
        public string NewProperty { get; set; }
    }
}
