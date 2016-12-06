using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.MvcSample.Models
{
    [LocalizedModel]
    public class BaseViewModel
    {
        [Display(Name = "This is message")]
        [Required(ErrorMessage = "Please provide valid message!")]
        [StringLength(100, MinimumLength = 5)]
        public string Message { get; set; }

        [Display(Name = "Base username:", Description = "")]
        [StringLength(100, MinimumLength = 5)]
        [UIHint("Username")]
        [HelpText]
        [FancyHelpText]
        public string BaseUsername { get; set; }

        public string CustomMessage { get; } = "Resource like property on base view model";
    }

    [LocalizedModel(Inherited = false)]
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Address = new Address();
        }

        [Display(Name = "The user name:", Description = "")]
        [Required]
        [UIHint("Username")]
        [HelpText]
        public string Username { get; set; }

        public Address Address { get; set; }
    }

    [LocalizedModel]
    public class Address
    {
        [UIHint("Street")]
        public string Street { get; set; }
    }
}
