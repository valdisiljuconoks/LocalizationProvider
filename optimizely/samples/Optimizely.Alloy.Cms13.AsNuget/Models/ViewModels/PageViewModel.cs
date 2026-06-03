using Optimizely.Alloy.Cms13.Models.Pages;

namespace Optimizely.Alloy.Cms13.Models.ViewModels;

public class PageViewModel<T>(T currentPage) : IPageViewModel<T> where T : SitePageData
{
    public T CurrentPage { get; private set; } = currentPage;

    public LayoutModel Layout { get; set; }

    public IContent Section { get; set; }
}

public static class PageViewModel
{
    /// <summary>
    /// Returns a PageViewModel of type <typeparam name="T"/>.
    /// </summary>
    /// <remarks>
    /// Convenience method for creating PageViewModels without having to specify the type as methods can use type inference while constructors cannot.
    /// </remarks>
    public static PageViewModel<T> Create<T>(T page)
        where T : SitePageData => new(page);
}
