using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelWithValidationClassAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedModelWithValidationClassAndNamespace
    {
        [Required]
        public string NewProperty { get; set; }
    }
}
