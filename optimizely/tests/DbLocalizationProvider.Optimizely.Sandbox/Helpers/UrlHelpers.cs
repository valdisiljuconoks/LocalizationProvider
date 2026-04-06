using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Optimizely.Sandbox.Helpers;

public static class UrlHelpers
{
    /// <summary>
    /// Returns the URL anchor target for a page with shortcut settings
    /// </summary>
    public static string PageLinkTarget(this IUrlHelper urlHelper, PageData page)
    {
        return page.LinkType switch
        {
            PageShortcutType.Normal => "",
            PageShortcutType.Inactive => "",
            PageShortcutType.FetchData => page.TargetFrameName,
            PageShortcutType.Shortcut => page.TargetFrameName,
            PageShortcutType.External => page.TargetFrameName,
            _ => throw new ArgumentOutOfRangeException($"Unknown link type: {page.LinkType}'")
        };
    }
}
