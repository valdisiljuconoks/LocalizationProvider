using AlloySampleSite.Controllers;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AlloySampleSite.Helpers
{
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
        {
            return viewContext.IsInEditMode() && (viewContext.ActionDescriptor as ControllerActionDescriptor)?.ControllerName == "Preview";
        }

        /// <summary>
        /// Determines if the request context is in edit mode.
        /// </summary>
        /// <param name="viewContext">The request context</param>
        /// <returns><code>true</code>If the context is in edit mode; otherwise <code>false</code></returns>
        public static bool IsInEditMode(this ViewContext viewContext)
        {
            var mode = viewContext.HttpContext.RequestServices.GetRequiredService<IContextModeResolver>().CurrentMode;
            return mode == ContextMode.Edit || mode == ContextMode.Preview;
        }
    }
}

