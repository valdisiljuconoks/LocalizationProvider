using AlloySampleSite.Models.Pages;
using AlloySampleSite.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;

namespace AlloySampleSite.Business
{
    [ServiceConfiguration]
    public class PageViewContextFactory
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly IDatabaseMode _databaseMode;
        private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;

        public PageViewContextFactory(IContentLoader contentLoader, UrlResolver urlResolver, IDatabaseMode databaseMode, IOptionsMonitor<CookieAuthenticationOptions> optionMonitor)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _databaseMode = databaseMode;
            _cookieAuthenticationOptions = optionMonitor.Get(IdentityConstants.ApplicationScheme);
        }

        public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, HttpContext httpContext)
        {
            var startPageContentLink = SiteDefinition.Current.StartPage;

            // Use the content link with version information when editing the startpage,
            // otherwise the published version will be used when rendering the props below.
            if (currentContentLink.CompareToIgnoreWorkID(startPageContentLink))
            {
                startPageContentLink = currentContentLink;
            }

            var startPage = _contentLoader.Get<StartPage>(startPageContentLink);

            return new LayoutModel
            {
                Logotype = startPage.SiteLogotype,
                LogotypeLinkUrl = new HtmlString(_urlResolver.GetUrl(SiteDefinition.Current.StartPage)),
                ProductPages = startPage.ProductPageLinks,
                CompanyInformationPages = startPage.CompanyInformationPageLinks,
                NewsPages = startPage.NewsPageLinks,
                CustomerZonePages = startPage.CustomerZonePageLinks,
                LoggedIn = httpContext.User.Identity.IsAuthenticated,
                LoginUrl = new HtmlString(GetLoginUrl(currentContentLink)),
                SearchActionUrl = new HtmlString(EPiServer.Web.Routing.UrlResolver.Current.GetUrl(startPage.SearchPageLink)),
                IsInReadonlyMode = _databaseMode.DatabaseMode == DatabaseMode.ReadOnly
            };
        }

        private string GetLoginUrl(ContentReference returnToContentLink)
        {
            return string.Format(
                "{0}?ReturnUrl={1}",
                _cookieAuthenticationOptions?.LoginPath.Value ?? VirtualPathResolver.Instance.ToAbsolute(Global.AppRelativeLoginPath),
                _urlResolver.GetUrl(returnToContentLink));
        }

        public virtual IContent GetSection(ContentReference contentLink)
        {
            var currentContent = _contentLoader.Get<IContent>(contentLink);
            if (currentContent.ParentLink != null && currentContent.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
            {
                return currentContent;
            }

            return _contentLoader.GetAncestors(contentLink)
                .OfType<PageData>()
                .SkipWhile(x => x.ParentLink == null || !x.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
                .FirstOrDefault();
        }
    }
}
