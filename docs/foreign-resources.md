# Tranlating Resources From Other ("foreign") Assmeblies

Good [friend and colleague](http://marisks.net/) of mine asked once: "How do I register localized resources for [ValidationIssue](http://world.episerver.com/documentation/Items/Developers-Guide/Episerver-Commerce/9/Orders/order-processing/) enum from EPiServer Commerce assembly?" Short answer back then was - you can't.
Until now.

With latest database localization provider now you can tell provider to get familiar with so called foreign resources and include those in sync process as well - even they are not decorated with `[LocalizedResource]` attribute. Actually attribute is only needed for provider to scan through and understand which types to include in sync process and which not.

Let's assume for some reason you need to localize foreign resource (`EPiServer.Core.VersionStatus`) for what you don't have source code and you cannot decorate type with required attributes. You will need to add similar code to register foreign resources:

```csharp
using System.Globalization;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace DbLocalizationProvider.Sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDbLocalizationProvider(ctx =>
           {
               ....
               ctx.ForeignResources.Add(typeof(ForeignResources));
           });
        }
    }
}
```

Using `ForeignResources` collection you are telling provider to include those types in resource sync process.
There are couple of extension methods as well for your convenience helping to add types to the collection.

Still you are able to use newly registered foreign resource in the same way as the rest of resources:

```csharp
[LocalizedModel]
public class MyCustomViewModel
{
    public VersionStatus Status { get; set; }
}



@model MyCustomViewModel

...
@Html.TranslateFor(m => m.Status)
```

Resource key naming conventions are the same as for ordinary discovered resources via `[LocalizedResource]` attribute.
