using DbLocalizationProvider.Optimizely.Sandbox.Models.Media;
using DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Optimizely.Sandbox.Components;

/// <summary>
/// Controller for the image file.
/// </summary>
public class ImageFileViewComponent(UrlResolver urlResolver) : PartialContentComponent<ImageFile>
{

    /// <summary>
    /// The index action for the image file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current image file.</param>
    protected override IViewComponentResult InvokeComponent(ImageFile currentContent)
    {
        var model = new ImageViewModel
        {
            Url = urlResolver.GetUrl(currentContent.ContentLink),
            Name = currentContent.Name,
            Copyright = currentContent.Copyright
        };

        return View(model);
    }
}
