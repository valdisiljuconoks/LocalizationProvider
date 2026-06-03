using Optimizely.Alloy.Cms13.Models.Media;
using Optimizely.Alloy.Cms13.Models.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Optimizely.Alloy.Cms13.Components;

/// <summary>
/// Controller for the video file.
/// </summary>
public class VideoFileViewComponent(UrlResolver urlResolver) : PartialContentComponent<VideoFile>
{

    /// <summary>
    /// The index action for the video file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current video file.</param>
    protected override IViewComponentResult InvokeComponent(VideoFile currentContent)
    {
        var model = new VideoViewModel
        {
            Url = urlResolver.GetUrl(currentContent.ContentLink),
            PreviewImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage)
                ? null
                : urlResolver.GetUrl(currentContent.PreviewImage),
        };

        return View(model);
    }
}
