using System.Web;
using System.Web.Caching;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public interface IResourceListProvider
    {
        string GetJson(string filename, HttpContext context, string languageName, bool debugMode);
        CacheDependency GetCacheDependency();
    }
}
