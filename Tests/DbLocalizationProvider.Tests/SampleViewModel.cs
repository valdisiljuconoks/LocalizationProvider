using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Tests
{
    [LocalizedModel]
    public class SampleViewModel
    {
        [DisplayName]
        [Required]
        [StringLength(100)]
        public string SampleProperty { get; set; }

        [Ignore]
        public string IgnoredProperty { get; set; }

        [Display(Name = "This is Display value")]
        [Required(ErrorMessage = "This is RequiredAttribute default error message")]
        public string SampleProperty2 { get; set; }

        public SubViewModel SubProperty { get; set; }

        [Include]
        public SubNonLocalizedViewModel ComplexIncludedProperty { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime? DateProperty { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int? NullableInt { get; set; }
    }

    public class SubNonLocalizedViewModel { }

    [LocalizedModel]
    public class SubViewModel
    {
        public string AnotherProperty { get; set; }
    }
}
