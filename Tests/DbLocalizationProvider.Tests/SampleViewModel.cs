using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests
{
    [LocalizedModel]
    public class SampleViewModel
    {
        [DisplayName]
        [Required]
        [StringLength(100)]
        public string SampleProperty { get; set; }

        [Display(Name = "This is Display value")]
        [Required(ErrorMessage = "This is RequiredAttribute default error message")]
        public string SampleProperty2 { get; set; }

        public SubViewModel SubProperty { get; set; }
    }

    public class SubViewModel
    {
        public string AnotherProperty { get; set; }
    }
}
