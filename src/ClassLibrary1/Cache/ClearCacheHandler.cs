using System.Collections.Generic;
using System.Web;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.AspNet.Cache
{
    public class ClearCacheHandler  : ICommandHandler<ClearCache.Command>
    {
        public void Execute(ClearCache.Command command)
        {
            if(HttpContext.Current == null)
                return;

            if(HttpContext.Current.Cache == null)
                return;

            var itemsToRemove = new List<string>();
            var enumerator = HttpContext.Current.Cache.GetEnumerator();

            while(enumerator.MoveNext())
            {
                if(enumerator.Key.ToString().ToLower().StartsWith(CacheKeyHelper.CacheKeyPrefix.ToLower()))
                    itemsToRemove.Add(enumerator.Key.ToString());
            }

            foreach(var itemToRemove in itemsToRemove)
                ConfigurationContext.Current.CacheManager.Remove(itemToRemove);
        }

    }
}
