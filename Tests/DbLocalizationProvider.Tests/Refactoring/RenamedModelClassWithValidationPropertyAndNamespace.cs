using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClassWithValidationPropertyAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedModelClassWithValidationPropertyAndNamespace
    {
        [Required]
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
    
    [LocalizedModel]
    [RenamedResource(OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedModelWithValidationPropertyAndNamespace
    {
        [Required]
        [RenamedResource("OldProperty")]
        public string NewProperty { get; set; }
    }
}
