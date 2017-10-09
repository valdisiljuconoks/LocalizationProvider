using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClassWithValidationProperty")]
    public class RenamedModelClassWithValidationProperty
    {
        [Required]
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
}
