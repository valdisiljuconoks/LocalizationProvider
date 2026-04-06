using DbLocalizationProvider.Optimizely.Sandbox.Business;
using DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;
using DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Optimizely.Sandbox.Controllers;

/// <summary>
/// All controllers that renders pages should inherit from this class so that we can
/// apply action filters, such as for output caching site wide, should we want to.
/// </summary>
public abstract class PageControllerBase<T> : PageController<T>, IModifyLayout
    where T : SitePageData
{
    protected readonly Injected<UISignInManager> UISignInManager;

    /// <summary>
    /// Signs out the current user and redirects to the Index action of the same controller.
    /// </summary>
    /// <remarks>
    /// There's a log out link in the footer which should redirect the user to the same page.
    /// As we don't have a specific user/account/login controller but rely on the login URL for
    /// forms authentication for login functionality we add an action for logging out to all
    /// controllers inheriting from this class.
    /// </remarks>
    public async Task<IActionResult> Logout()
    {
        await UISignInManager.Service.SignOutAsync();
        return Redirect(HttpContext.RequestServices.GetService<UrlResolver>().GetUrl(PageContext.ContentLink, PageContext.LanguageID));
    }

    public virtual void ModifyLayout(LayoutModel layoutModel)
    {
        if (PageContext.Content is SitePageData page)
        {
            layoutModel.HideHeader = page.HideSiteHeader;
            layoutModel.HideFooter = page.HideSiteFooter;
        }
    }
}
