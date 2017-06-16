# Cache Events

Got a request after one of my presentation from audience to expose events around cache - when item is added to the cache, when removed, etc. Reason for this was manual cache event propogation further to other nodes in the cluster.

Now you are able to subscribe to cache events via `ConfigurationContext`:

```
[assembly: OwinStartup(typeof(Startup))]

namespace DbLocalizationProvider.MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDbLocalizationProvider(ctx =>
            {
                ...
                ctx.CacheManager.OnRemove += CacheManagerOnOnRemove;
            });
        }

        private void CacheManagerOnOnRemove(CacheEventArgs args)
        { 
            // black magic happens here..
        }
    }
}
```

You will be able to get info about cache event (operation - `Insert`, `Remove`, etc) via `CacheEventArgs` argument.
