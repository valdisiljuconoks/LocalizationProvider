using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.Core.AspNetSample.Models
{
    [LocalizedModel]
    public class UserViewModel
    {
        [Display(Name = "User name:")]
        [Required(ErrorMessage = "Name of the user is required!")]
        public string UserName { get; set; }
    }
}
