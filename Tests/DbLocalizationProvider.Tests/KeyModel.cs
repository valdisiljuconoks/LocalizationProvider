using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests
{
    [LocalizedModel]
    public class KeyModel
    {
        public KeyModel()
        {
            SubKeyProperty = new SubKeyModel();
        }

        [DisplayName("Default display name for SampleProperty")]
        [Required]
        public string SampleProperty { get; set; }

        public SubKeyModel SubKeyProperty { get; set; }

        public static string ThisIsConstant => "Default value for constant";
    }
}
