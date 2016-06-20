using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.MvcSample.Models
{
    [LocalizedModel]
    public class HomeViewModel
    {
        [Required]
        public string Username { get; set; }
    }
}
