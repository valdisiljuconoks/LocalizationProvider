using AlloySampleSite.Models.Media;
using AlloySampleSite.Models.ViewModels;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AlloySampleSite.Controllers
{
    /// <summary>
    /// Controller for the video file.
    /// </summary>
    public class VideoFileViewComponent : PartialContentComponent<VideoFile>
    {
        private readonly UrlResolver _urlResolver;

        public VideoFileViewComponent(UrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// The index action for the video file. Creates the view model and renders the view.
        /// </summary>
        /// <param name="currentContent">The current video file.</param>
        protected override IViewComponentResult InvokeComponent(VideoFile currentContent)
        {
            var model = new VideoViewModel
            {
                Url = _urlResolver.GetUrl(currentContent.ContentLink),
                PreviewImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage) ? String.Empty : _urlResolver.GetUrl(currentContent.PreviewImage),
            };

            return View(model);
        }
    }
}
