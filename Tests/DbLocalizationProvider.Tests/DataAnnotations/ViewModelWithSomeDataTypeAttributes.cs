using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    [LocalizedModel]
    public class ViewModelWithSomeDataTypeAttributes
    {
        [Display(Name = "Some Poprerty")]
        [DataType(DataType.PhoneNumber)]
        public string SomePorperty { get; set; }
    }
}
