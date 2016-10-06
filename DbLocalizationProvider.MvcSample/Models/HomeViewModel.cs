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

        public string CustomMessage { get; } = "Resource like property on base view model";
    }

    [LocalizedModel(Inherited = false)]
    public class HomeViewModel : BaseViewModel
    {
        [Display(Name = "The user name:")]
        [Required]
        public string Username { get; set; }
    }
}
