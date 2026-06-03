using Optimizely.Alloy.Cms13.Models.Pages;

namespace Optimizely.Alloy.Cms13.Models.ViewModels;

public class PreviewModel(
    SitePageData currentPage,
    IContent previewContent) : PageViewModel<SitePageData>(currentPage)
{
    public IContent PreviewContent { get; set; } = previewContent;

    public List<PreviewArea> Areas { get; set; } = [];

    public class PreviewArea
    {
        public bool Supported { get; set; }

        public string AreaName { get; set; }

        public string AreaTag { get; set; }

        public ContentArea ContentArea { get; set; }
    }
}
