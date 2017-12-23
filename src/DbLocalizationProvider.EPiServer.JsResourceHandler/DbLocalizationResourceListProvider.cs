using System.Web;
using System.Web.Caching;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using Newtonsoft.Json;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public class DbLocalizationResourceListProvider : IResourceListProvider
    {
        private readonly ResourceJsonConverter _converter;
        private readonly ResourceFilter _filter;

        public DbLocalizationResourceListProvider(ResourceFilter filter, ResourceJsonConverter converter)
        {
            _filter = filter;
            _converter = converter;
        }

        public string GetJson(string filename, HttpContext context, string languageName, bool debugMode)
        {
            // for us - filename it's actually root namespace of the resource key to retrieve
            var resources = new GetAllResources.Query().Execute();
            var filteredResources = _filter.GetResourcesWithStartingKey(resources, filename);

            // we need to process key names and supported tested classes with "+" symbols in keys -> so we replace those wiyh dots to have proper object nesting on client side
            filteredResources.ForEach(r => r.ResourceKey = r.ResourceKey.Replace("+", "."));

            return JsonConvert.SerializeObject(_converter.Convert(filteredResources, languageName),
                debugMode ? Formatting.Indented : Formatting.None);
        }

        public CacheDependency GetCacheDependency()
        {
            return null;
        }
    }
}
