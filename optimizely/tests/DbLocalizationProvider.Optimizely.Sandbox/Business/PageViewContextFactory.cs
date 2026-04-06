using DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;
using DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;
using EPiServer.Data;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Optimizely.Sandbox.Business;

[ServiceConfiguration]
public class PageViewContextFactory(
    IContentLoader contentLoader,
    UrlResolver urlResolver,
    IDatabaseMode databaseMode,
    IOptionsMonitor<CookieAuthenticationOptions> optionMonitor)
{
    private readonly CookieAuthenticationOptions _cookieAuthenticationOptions = optionMonitor.Get(IdentityConstants.ApplicationScheme);

    public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, HttpContext httpContext)
    {
        var startPageContentLink = ContentReference.StartPage;

        // Use the content link with version information when editing the startpage,
        // otherwise the published version will be used when rendering the props below.
        if (currentContentLink.CompareToIgnoreWorkID(startPageContentLink))
        {
            startPageContentLink = currentContentLink;
        }

        var startPage = contentLoader.Get<StartPage>(startPageContentLink);

        return new LayoutModel
        {
            Logotype = startPage.SiteLogotype,
            LogotypeLinkUrl = new HtmlString(urlResolver.GetUrl(ContentReference.StartPage)),
            ProductPages = startPage.ProductPageLinks,
            CompanyInformationPages = startPage.CompanyInformationPageLinks,
            NewsPages = startPage.NewsPageLinks,
            CustomerZonePages = startPage.CustomerZonePageLinks,
            LoggedIn = httpContext.User.Identity.IsAuthenticated,
            LoginUrl = new HtmlString(GetLoginUrl(currentContentLink)),
            SearchActionUrl = new HtmlString(UrlResolver.Current.GetUrl(startPage.SearchPageLink)),
            IsInReadonlyMode = databaseMode.DatabaseMode == DatabaseMode.ReadOnly
        };
    }

    private string GetLoginUrl(ContentReference returnToContentLink)
    {
        return $"{_cookieAuthenticationOptions?.LoginPath.Value ?? Globals.LoginPath}?ReturnUrl={urlResolver.GetUrl(returnToContentLink)}";
    }

    public virtual IContent GetSection(ContentReference contentLink)
    {
        var currentContent = contentLoader.Get<IContent>(contentLink);

        static bool isSectionRoot(ContentReference contentReference) =>
           ContentReference.IsNullOrEmpty(contentReference) ||
           contentReference.Equals(ContentReference.StartPage) ||
           contentReference.Equals(ContentReference.RootPage);

        if (isSectionRoot(currentContent.ParentLink))
        {
            return currentContent;
        }

        return contentLoader.GetAncestors(contentLink)
            .OfType<PageData>()
            .SkipWhile(x => !isSectionRoot(x.ParentLink))
            .FirstOrDefault();
    }
}
