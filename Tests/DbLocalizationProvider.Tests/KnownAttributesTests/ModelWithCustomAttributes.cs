using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    [LocalizedModel]
    public class ModelWithCustomAttributes
    {
        [Display(Name = "User name")]
        [Required]
        [StringLength(5)]
        [HelpText]
        public string UserName { get; set; }
    }

    [LocalizedModel(Inherited = false)]
    public class ChildModelWithCustomAttributes : ModelWithCustomAttributes
    {
        [HelpText]
        public string FirstName { get; set; }

        [HelpText]
        public string LastName { get; set; }
    }

    [LocalizedModel]
    public class ModelWithTwoChildModelPropertiesCustomAttributes
    {
        public ChildModelWithCustomAttributes AsFirst { get; set; }

        public ChildModelWithCustomAttributes AsSecond { get; set; }
    }

    [LocalizedModel]
    public class AnotherModelWithTwoChildModelPropertiesCustomAttributes
    {
        public ChildModelWithCustomAttributes AsThird { get; set; }

        public ChildModelWithCustomAttributes AsFourth { get; set; }
    }
}
