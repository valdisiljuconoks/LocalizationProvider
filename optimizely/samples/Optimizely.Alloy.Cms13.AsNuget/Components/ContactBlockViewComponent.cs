using Optimizely.Alloy.Cms13.Models.Blocks;
using Optimizely.Alloy.Cms13.Models.Pages;
using Optimizely.Alloy.Cms13.Models.ViewModels;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Optimizely.Alloy.Cms13.Components;

public class ContactBlockViewComponent(
    IContentLoader contentLoader,
    IPermanentLinkMapper permanentLinkMapper) : BlockComponent<ContactBlock>
{
    protected override IViewComponentResult InvokeComponent(ContactBlock currentContent)
    {
        ContactPage contactPage = null;
        if (!ContentReference.IsNullOrEmpty(currentContent.ContactPageLink))
        {
            contactPage = contentLoader.Get<ContactPage>(currentContent.ContactPageLink);
        }

        var linkUrl = GetLinkUrl(currentContent);

        var model = new ContactBlockModel
        {
            Heading = currentContent.Heading,
            Image = currentContent.Image,
            ContactPage = contactPage,
            LinkUrl = GetLinkUrl(currentContent),
            LinkText = currentContent.LinkText,
            ShowLink = linkUrl != null
        };

        // As we're using a separate view model with different property names than the content object
        // we connect the view models properties with the content objects so that they can be edited.
        ViewData.GetEditHints<ContactBlockModel, ContactBlock>()
            .AddConnection(x => x.Heading, x => x.Heading)
            .AddConnection(x => x.Image, x => x.Image)
            .AddConnection(x => (object)x.ContactPage, x => x.ContactPageLink)
            .AddConnection(x => x.LinkText, x => x.LinkText);

        return View(model);
    }

    private HtmlString GetLinkUrl(ContactBlock contactBlock)
    {
        if (contactBlock.LinkUrl != null && !contactBlock.LinkUrl.IsEmpty())
        {
            var linkUrl = contactBlock.LinkUrl.ToString();

            // If the url maps to a page on the site we convert it from the internal (permanent, GUID-like) format
            // to the human readable and pretty public format
            var linkMap = permanentLinkMapper.Find(new UrlBuilder(linkUrl));
            if (linkMap != null && !ContentReference.IsNullOrEmpty(linkMap.ContentReference))
            {
                return new HtmlString(Url.ContentUrl(linkMap.ContentReference));
            }

            return new HtmlString(contactBlock.LinkUrl.ToString());
        }

        return null;
    }
}
