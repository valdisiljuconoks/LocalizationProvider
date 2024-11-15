using AlloySampleSite.Models.Pages;
using EPiServer.Core;
using System.Collections.Generic;

namespace AlloySampleSite.Models.ViewModels
{
    public class PreviewModel : PageViewModel<SitePageData>
    {
        public PreviewModel(SitePageData currentPage, IContent previewContent)
            : base(currentPage)
        {
            PreviewContent = previewContent;
            Areas = new List<PreviewArea>();
        }

        public IContent PreviewContent { get; set; }
        public List<PreviewArea> Areas { get; set; }

        public class PreviewArea
        {
            public bool Supported { get; set; }
            public string AreaName { get; set; }
            public string AreaTag { get; set; }
            public ContentArea ContentArea { get; set; }
        }
    }
}