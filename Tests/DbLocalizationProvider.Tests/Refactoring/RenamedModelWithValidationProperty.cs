using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    public class RenamedModelWithValidationProperty
    {
        [Required]
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
}
