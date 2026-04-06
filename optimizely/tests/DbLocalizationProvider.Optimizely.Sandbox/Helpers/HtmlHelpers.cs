using System.Text;
using System.Text.Encodings.Web;
using DbLocalizationProvider.Optimizely.Sandbox.Business;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DbLocalizationProvider.Optimizely.Sandbox.Helpers;

public static class HtmlHelpers
{
    /// <summary>
    /// Returns an element for each child page of the rootLink using the itemTemplate.
    /// </summary>
    /// <param name="helper">The html helper in whose context the list should be created</param>
    /// <param name="rootLink">A reference to the root whose children should be listed</param>
    /// <param name="itemTemplate">A template for each page which will be used to produce the return value. Can be either a delegate or a Razor helper.</param>
    /// <param name="includeRoot">Wether an element for the root page should be returned</param>
    /// <param name="requireVisibleInMenu">Wether pages that do not have the "Display in navigation" checkbox checked should be excluded</param>
    /// <param name="requirePageTemplate">Wether page that do not have a template (i.e. container pages) should be excluded</param>
    /// <remarks>
    /// Filter by access rights and publication status.
    /// </remarks>
    public static IHtmlContent MenuList(
        this IHtmlHelper helper,
        ContentReference rootLink,
        Func<MenuItem, HelperResult> itemTemplate = null,
        bool includeRoot = false,
        bool requireVisibleInMenu = true,
        bool requirePageTemplate = true)
    {
        itemTemplate ??= GetDefaultItemTemplate(helper);
        var currentContentLink = helper.ViewContext.HttpContext.GetContentLink();
        var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

        IEnumerable<PageData> filter(IEnumerable<PageData> pages)
            => pages.FilterForDisplay(requirePageTemplate, requireVisibleInMenu);

        var pagePath = contentLoader.GetAncestors(currentContentLink)
            .Reverse()
            .Select(x => x.ContentLink)
            .SkipWhile(x => !x.CompareToIgnoreWorkID(rootLink))
            .ToList();

        var menuItems = contentLoader.GetChildren<PageData>(rootLink)
            .FilterForDisplay(requirePageTemplate, requireVisibleInMenu)
            .Select(x => CreateMenuItem(x, currentContentLink, pagePath, contentLoader, filter))
            .ToList();

        if (includeRoot)
        {
            menuItems.Insert(0, CreateMenuItem(contentLoader.Get<PageData>(rootLink), currentContentLink, pagePath, contentLoader, filter));
        }

        var buffer = new StringBuilder();
        var writer = new StringWriter(buffer);
        foreach (var menuItem in menuItems)
        {
            itemTemplate(menuItem).WriteTo(writer, HtmlEncoder.Default);
        }

        return new HtmlString(buffer.ToString());
    }

    private static MenuItem CreateMenuItem(PageData page, ContentReference currentContentLink, List<ContentReference> pagePath, IContentLoader contentLoader, Func<IEnumerable<PageData>, IEnumerable<PageData>> filter)
    {
        var menuItem = new MenuItem(page)
        {
            Selected = page.ContentLink.CompareToIgnoreWorkID(currentContentLink) ||
                       pagePath.Contains(page.ContentLink),

            HasChildren = new Lazy<bool>(() => filter(contentLoader.GetChildren<PageData>(page.ContentLink)).Any())
        };

        return menuItem;
    }

    private static Func<MenuItem, HelperResult> GetDefaultItemTemplate(IHtmlHelper helper)
    {
        return x => new HelperResult(writer =>
        {
            helper.PageLink(x.Page).WriteTo(writer, HtmlEncoder.Default);
            return Task.CompletedTask;
        });
    }

    public class MenuItem(PageData page)
    {
        public PageData Page { get; set; } = page;

        public bool Selected { get; set; }

        public Lazy<bool> HasChildren { get; set; }
    }

    /// <summary>
    /// Writes an opening <![CDATA[ <a> ]]> tag to the response if the shouldWriteLink argument is true.
    /// Returns a ConditionalLink object which when disposed will write a closing <![CDATA[ </a> ]]> tag
    /// to the response if the shouldWriteLink argument is true.
    /// </summary>
    public static ConditionalLink BeginConditionalLink(this IHtmlHelper helper, bool shouldWriteLink, string url,
        string title = null, string cssClass = null, string linkTarget = null)
    {
        if (shouldWriteLink)
        {
            var linkTag = new TagBuilder("a");
            linkTag.Attributes.Add("href", url);

            if (!string.IsNullOrWhiteSpace(title))
            {
                linkTag.Attributes.Add("title", title);
            }

            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                linkTag.Attributes.Add("class", cssClass);
            }

            if (!string.IsNullOrWhiteSpace(linkTarget))
            {
                linkTag.Attributes.Add("target", linkTarget);
            }

            helper.ViewContext.Writer.Write(linkTag.RenderStartTag());
        }
        return new ConditionalLink(helper.ViewContext, shouldWriteLink);
    }

    /// <summary>
    /// Writes an opening <![CDATA[ <a> ]]> tag to the response if the shouldWriteLink argument is true.
    /// Returns a ConditionalLink object which when disposed will write a closing <![CDATA[ </a> ]]> tag
    /// to the response if the shouldWriteLink argument is true.
    /// </summary>
    /// <remarks>
    /// Overload which only executes the delegate for retrieving the URL if the link should be written.
    /// This may be used to prevent null reference exceptions by adding null checkes to the shouldWriteLink condition.
    /// </remarks>
    public static ConditionalLink BeginConditionalLink(this IHtmlHelper helper, bool shouldWriteLink,
        Func<string> urlGetter, string title = null, string cssClass = null, string linkTarget = null)
    {
        var url = string.Empty;

        if (shouldWriteLink)
        {
            url = urlGetter();
        }

        return helper.BeginConditionalLink(shouldWriteLink, url, title, cssClass, linkTarget);
    }

    public class ConditionalLink(ViewContext viewContext, bool isLinked) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (isLinked)
            {
                viewContext.Writer.Write("</a>");
            }
        }
    }
}
