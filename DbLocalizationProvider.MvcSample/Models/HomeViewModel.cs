using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.MvcSample.Models
{
    [LocalizedModel]
    public class BaseViewModel
    {
        [Display(Name = "This is message")]
        public string Message { get; set; }
    }

    [LocalizedModel]
    public class HomeViewModel : BaseViewModel
    {
        [Required]
        public string Username { get; set; }
    }
}
