using EPiServer;
using EPiServer.Core;
using System;

namespace AlloySampleSite.Models.ViewModels
{
    public class ContentRenderingErrorModel
    {
        public ContentRenderingErrorModel(IContentData contentData, Exception exception)
        {
            var content = contentData as IContent;
            if (content != null)
            {
                ContentName = content.Name;
            }
            else
            {
                ContentName = string.Empty;
            }

            ContentTypeName = contentData.GetOriginalType().Name;

            Exception = exception;
        }

        public string ContentName { get; set; }
        public string ContentTypeName { get; set; }
        public Exception Exception { get; set; }
    }
}