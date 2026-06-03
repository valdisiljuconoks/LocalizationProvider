using Optimizely.Alloy.Cms13.Models.Pages;

namespace Optimizely.Alloy.Cms13.Models.ViewModels;

public class SearchContentModel(SearchPage currentPage) : PageViewModel<SearchPage>(currentPage)
{
    public bool SearchServiceDisabled { get; set; }

    public string SearchedQuery { get; set; }

    public int NumberOfHits { get; set; }

    public IEnumerable<SearchHit> Hits { get; set; }

    public class SearchHit
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Excerpt { get; set; }
    }
}
