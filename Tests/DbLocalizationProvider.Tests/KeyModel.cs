using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Tests
{
    public class KeyModel : ILocalizedModel
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
