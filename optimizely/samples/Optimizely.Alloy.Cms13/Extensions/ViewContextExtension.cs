using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Optimizely.Alloy.Cms13.Helpers;

/// <summary>
/// Extension methods on request Context such as et/Set Node, Lang, Controller
/// </summary>
public static class ViewContextExtension
{

    /// <summary>
    /// Determine if the the controller is in the preview mode.
    /// </summary>
    /// <param name="viewContext"></param>
    /// <returns></returns>
    public static bool IsPreviewMode(this ViewContext viewContext)
        => viewContext.IsInEditMode() && viewContext.ActionDescriptor is ControllerActionDescriptor { ControllerName: "Preview" };

    /// <summary>
    /// Determines if the request context is in edit mode.
    /// </summary>
    /// <param name="viewContext">The request context</param>
    /// <returns><code>true</code>If the context is in edit mode; otherwise <code>false</code></returns>
    public static bool IsInEditMode(this ViewContext viewContext)
    {
        var mode = viewContext.HttpContext.RequestServices.GetRequiredService<IContextModeResolver>().CurrentMode;
        return mode is ContextMode.Edit or ContextMode.Preview;
    }
}

