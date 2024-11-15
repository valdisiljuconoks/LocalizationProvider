using AlloySampleSite.Helpers;
using AlloySampleSite.Models.Blocks;
using AlloySampleSite.Models.Pages;
using AlloySampleSite.Models.ViewModels;
using EPiServer;
using EPiServer.Cms.AspNetCore.Mvc;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace AlloySampleSite.Controllers
{
    public class ContactBlockViewComponent : BlockComponent<ContactBlock>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IPermanentLinkMapper _permanentLinkMapper;

        public ContactBlockViewComponent(IContentLoader contentLoader, IPermanentLinkMapper permanentLinkMapper)
        {
            _contentLoader = contentLoader;
            _permanentLinkMapper = permanentLinkMapper;
        }

        protected override IViewComponentResult InvokeComponent(ContactBlock currentBlock)
        {
            ContactPage contactPage = null;
            if (!ContentReference.IsNullOrEmpty(currentBlock.ContactPageLink))
            {
                contactPage = _contentLoader.Get<ContactPage>(currentBlock.ContactPageLink);
            }

            var linkUrl = GetLinkUrl(currentBlock);

            var model = new ContactBlockModel
            {
                Heading = currentBlock.Heading,
                Image = currentBlock.Image,
                ContactPage = contactPage,
                LinkUrl = GetLinkUrl(currentBlock),
                LinkText = currentBlock.LinkText,
                ShowLink = linkUrl != null
            };

            //As we're using a separate view model with different property names than the content object
            //we connect the view models properties with the content objects so that they can be edited.
            ViewData.GetEditHints<ContactBlockModel, ContactBlock>()
                .AddConnection(x => x.Heading, x => x.Heading)
                .AddConnection(x => x.Image, x => x.Image)
                .AddConnection(x => (object)x.ContactPage, x => (object)x.ContactPageLink)
                .AddConnection(x => x.LinkText, x => x.LinkText);

            return View(model);
        }

        private IHtmlContent GetLinkUrl(ContactBlock contactBlock)
        {
            if (contactBlock.LinkUrl != null && !contactBlock.LinkUrl.IsEmpty())
            {
                var linkUrl = contactBlock.LinkUrl.ToString();

                //If the url maps to a page on the site we convert it from the internal (permanent, GUID-like) format
                //to the human readable and pretty public format
                var linkMap = _permanentLinkMapper.Find(new UrlBuilder(linkUrl));
                if (linkMap != null && !ContentReference.IsNullOrEmpty(linkMap.ContentReference))
                {
                    return new HtmlString(Url.PageLinkUrl(linkMap.ContentReference));
                }

                return new HtmlString(contactBlock.LinkUrl.ToString());
            }

            return null;
        }

    }
}
