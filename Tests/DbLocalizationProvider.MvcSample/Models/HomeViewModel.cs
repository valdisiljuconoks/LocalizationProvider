using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.MvcSample.Resources;

namespace DbLocalizationProvider.MvcSample.Models
{
    [LocalizedModel]
    public class BaseViewModel
    {
        [Display(Name = "This is message")]
        [Required(ErrorMessage = "Please provide valid message!")]
        [StringLength(100, MinimumLength = 5)]
        public string Message { get; set; }

        [ResourceKey("the-resource-key")]
        [Display(Name = "[ResourceKey] property")]
        [Required(ErrorMessage = "Please provide valid resource key property value!")]
        public string ResourceKeyProperty { get; set; }

        [Display(Name = "Base username:", Description = "")]
        [StringLength(100, MinimumLength = 5)]
        [UIHint("Username")]
        [HelpText]
        [FancyHelpText]
        public string BaseUsername { get; set; }

        public string ThisIsBaseField = "This is base field";

        public string CustomMessage { get; } = "Resource like property on base view model";
    }

    [LocalizedModel(Inherited = false)]
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Address = new Address
            {
                Type = AddressType.Billing,
                CityType = CityType.Small
            };
        }

        [Display(Name = "The user name:", Description = "")]
        [Required]
        [UIHint("Username")]
        [HelpText]
        public string Username { get; set; }

        public string ThisIsField = "this is field";

        [Required]
        //[DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "You have to specify valid email address!!!")]
        public string Email { get; set; }

        public Address Address { get; set; }

        [UseResource(typeof(CommonResources), nameof(CommonResources.Ok))]
        public string Ok { get; set; }
    }

    [LocalizedModel]
    public class Address
    {
        [UIHint("Street")]
        public string Street { get; set; }

        public AddressType Type { get; set; }

        public CityType CityType { get; set; }
    }

    [LocalizedResource]
    public enum AddressType
    {
        None,
        Billing,
        Actual
    }

    [LocalizedResource(KeyPrefix = "/city/type")]
    public enum CityType
    {
        None,
        [ResourceKey("/big")]
        Big,
        [ResourceKey("/small")]
        Small
    }
}
