using System;
using System.Web;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public class HttpHandler : IHttpHandler
    {
        private Injected<IResourceListProvider> _provider;

        public void ProcessRequest(HttpContext context)
        {
            if(_provider.Service == null)
                throw new InvalidOperationException("Implementation of `IResourceListProvider` is not configured in IoC container.");

            var languageSelector = LanguageSelector.AutoDetect();
            var languageName = string.IsNullOrEmpty(context.Request.QueryString["lang"])
                                   ? languageSelector.Language.Name
                                   : context.Request.QueryString["lang"];

            var filename = ExtractFileName(context);

            var debugMode = context.Request.QueryString["debug"] != null;
            var alias = string.IsNullOrEmpty(context.Request.QueryString["alias"]) ? "jsl10n" : context.Request.QueryString["alias"];

            var cacheKey = $"{filename}_{languageName}_{(debugMode ? "debug" : "release")}}}";
            var responseObject = context.Cache.Get(cacheKey) as string;

            if(responseObject == null)
            {
                responseObject = _provider.Service.GetJson(filename, context, languageName, debugMode);
                responseObject = $"window.{alias} = {responseObject}";

                var dependency = _provider.Service.GetCacheDependency();
                if(dependency != null)
                    context.Cache.Insert(cacheKey, responseObject, dependency);
            }

            context.Response.Write(responseObject);
            context.Response.ContentType = "text/javascript";
        }

        public bool IsReusable { get; }

        private static string ExtractFileName(HttpContext context)
        {
            var result = context.Request.Path.Replace(Constants.PathBase, string.Empty);
            return result.StartsWith("/") ? result.TrimStart('/') : result;
        }
    }
}
