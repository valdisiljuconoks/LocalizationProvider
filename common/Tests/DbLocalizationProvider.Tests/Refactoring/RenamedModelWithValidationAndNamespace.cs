using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring;

[LocalizedModel]
[RenamedResource(OldNamespace = "In.Galaxy.Far.Far.Away")]
public class RenamedModelWithValidationAndNamespace
{
    [Required]
    public string NewProperty { get; set; }
}
