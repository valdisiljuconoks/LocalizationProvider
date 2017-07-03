using System.Collections.Generic;
using System.Web;
using DbLocalizationProvider.Cache;
using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class JsResourceCacheInvalidationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Current.CacheManager.OnRemove += CacheManagerOnOnRemove;
        }

        public void Uninitialize(InitializationEngine context)
        {
            ConfigurationContext.Current.CacheManager.OnRemove -= CacheManagerOnOnRemove;
        }

        private void CacheManagerOnOnRemove(CacheEventArgs cacheEventArgs)
        {
            var existingKeys = HttpContext.Current.Cache.GetEnumerator();
            var entriesToRemove = new List<string>();

            while (existingKeys.MoveNext())
            {
                var existingKey = CacheKeyHelper.GetContainerName(existingKeys.Key.ToString());
                if(existingKey!= null && cacheEventArgs.ResourceKey.StartsWith(existingKey))
                    entriesToRemove.Add(existingKey);
            }

            foreach (var entry in entriesToRemove)
                CacheManager.Remove(entry);
        }
    }
}
