using DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;
using DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Optimizely.Sandbox.Controllers;

public class SearchPageController : PageControllerBase<SearchPage>
{
    public ViewResult Index(SearchPage currentPage, string q)
    {
        var model = new SearchContentModel(currentPage)
        {
            Hits = Enumerable.Empty<SearchContentModel.SearchHit>(),
            NumberOfHits = 0,
            SearchServiceDisabled = true,
            SearchedQuery = q
        };

        return View(model);
    }
}
