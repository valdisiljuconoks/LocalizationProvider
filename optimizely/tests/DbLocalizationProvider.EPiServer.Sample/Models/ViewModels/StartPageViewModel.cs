using System;
using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.EPiServer.Sample.Models.Pages;

namespace DbLocalizationProvider.EPiServer.Sample.Models.ViewModels
{
    [LocalizedModel]
    public class StartPageViewModel
    {
        public StartPageViewModel(StartPage currentPage)
        {
            CurrentPage = currentPage ?? throw new ArgumentNullException(nameof(currentPage));
        }

        public StartPage CurrentPage { get; set; }

        [Display(Name = "User name", Description = "Please provide your username")]
        [Required(ErrorMessage ="User name is required")]
        public string Username { get; set; }

        [Display(Name = "/this/is/path")]
        public string XPathProperty { get; set; }
    }
}
