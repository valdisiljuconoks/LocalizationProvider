using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    [LocalizedModel]
    public class ViewModelWithInheritedDataTypeAttributes
    {
        [Display(Name = "Some Poprerty")]
        [EmailAddress]
        public string SomeProperty { get; set; }
    }
}
