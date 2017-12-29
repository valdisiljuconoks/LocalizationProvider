using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNetSample.Models
{
    [LocalizedModel]
    public class UserViewModel
    {
        public UserViewModel()
        {
            Address = new AddressViewModel();
        }

        public AddressViewModel Address { get; set; }

        [Display(Name = "User name:")]
        [Required(ErrorMessage = "Name of the user is required!")]
        public string UserName { get; set; }

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Password is kinda required :)")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Please use longer password than 5 symbols")]
        public string Password { get; set; }
    }
}
