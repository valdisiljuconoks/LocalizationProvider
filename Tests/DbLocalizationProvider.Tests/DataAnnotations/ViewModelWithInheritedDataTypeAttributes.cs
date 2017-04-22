using System.ComponentModel.DataAnnotations;

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
