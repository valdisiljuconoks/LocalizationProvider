using System;
using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.EPiServer.Sample.Models.Pages;

namespace DbLocalizationProvider.EPiServer.Sample.Models.ViewModels
{
    public class StartPageViewModel
    {
        public StartPageViewModel(StartPage currentPage)
        {
            if(currentPage == null)
                throw new ArgumentNullException(nameof(currentPage));

            CurrentPage = currentPage;
        }

        public StartPage CurrentPage { get; set; }

        [Display(Name = "User name", Description = "Please provide your username")]
        public string Username { get; set; }
    }
}
