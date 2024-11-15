using AlloySampleSite.Models.Pages;
using AlloySampleSite.Models.ViewModels;
using DbLocalizationProvider;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace AlloySampleSite.Controllers;

public class StartPageController(ILocalizationProvider localizationProvider) : PageControllerBase<StartPage>
{
    public IActionResult Index(StartPage currentPage)
    {
        var model = PageViewModel.Create(currentPage);

        if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink)) // Check if it is the StartPage or just a page of the StartPage type.
        {
            //Connect the view models logotype property to the start page's to make it editable
            var editHints = ViewData.GetEditHints<PageViewModel<StartPage>, StartPage>();
            editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
            editHints.AddConnection(m => m.Layout.ProductPages, p => p.ProductPageLinks);
            editHints.AddConnection(m => m.Layout.CompanyInformationPages, p => p.CompanyInformationPageLinks);
            editHints.AddConnection(m => m.Layout.NewsPages, p => p.NewsPageLinks);
            editHints.AddConnection(m => m.Layout.CustomerZonePages, p => p.CustomerZonePageLinks);
        }

        // retrieve non-existing resource
        var x = localizationProvider.GetString("adsfadsfasdf");

        return View(model);
    }
}
