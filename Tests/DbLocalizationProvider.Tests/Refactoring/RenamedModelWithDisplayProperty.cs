using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelWithDisplayProperty")]
    public class RenamedModelWithDisplayProperty
    {
        [Display(Description = "some text")]
        public string NewProperty { get; set; }
    }
}
