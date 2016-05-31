using System.Collections.Generic;
using System.Web;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Commands
{
    public class ClearCache
    {
        public class Command : ICommand { }

        public class Handler : ICommandHandler<Command>
        {
            public void Execute(Command command)
            {
                if(HttpContext.Current == null)
                    return;

                if(HttpContext.Current.Cache == null)
                    return;

                var itemsToRemove = new List<string>();
                var enumerator = HttpContext.Current.Cache.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if(enumerator.Key.ToString().ToLower().StartsWith(CacheKeyHelper.CacheKeyPrefix.ToLower()))
                    {
                        itemsToRemove.Add(enumerator.Key.ToString());
                    }
                }

                foreach (var itemToRemove in itemsToRemove)
                {
                    ConfigurationContext.Current.CacheManager.Remove(itemToRemove);
                }
            }
        }
    }
}
