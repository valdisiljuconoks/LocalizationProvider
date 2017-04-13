Along with other smaller bug fixes, database-driven localization provider for EPiServer got closer to front-end. We joined forces together with my old pal and friend [Arve Systad](https://github.com/ArveSystad) and made it possible to add translations to client side resources as well.

## Setup

So setup for the client-side resource localization with help of DbLocalizationProvider plugin is more or less straight forward. You will need to install `DbLocalizationProvider.EPiServer.JsResourceHandler` package from [EPiServer feed](http://nuget.episerver.com/en/?search=localization) and add corresponding `<script>` include in your markup file to fetch translations from the server:

```
@Html.GetTranslations(typeof(...{type of the class holding resources}...))
```

or

```
<script src="/jsl10n/{beginning-of-resource-keys}">
```

You have to specify beginning of resource keys to fetch those from the server.


### Sample Resources

For instance you have following resources defined in your code:

```csharp
namespace MyProject
{
    [LocalizedResource]
    public class MyResources
    {
        public static string FirstProperty => "One";
        public static string SecondProperty => "Two";
    }

    [LocalizedResource]
    public class AlternativeResources
    {
        public static string ThirdProperty => "Three";
        public static string FothProperty => "Four";
    }
}
```

Following resources will be registered:

```
MyProject.MyResources.FirstProperty
MyProject.MyResources.SecondProperty 
MyProject.AlternativeResources.ThirdProperty 
MyProject.AlternativeResources.FothProperty 
```

If we include following code:

```
@Html.GetTranslations(typeof(MyProject))
```

or following `<script>` element:

```html
<script src="/jsl10n/MyProject"></script>
```

All resources from both classes will be retrieved:

<img src="http://blog.tech-fellow.net/content/images/2017/03/2017-03-19_23-54-26.png"/>

Resources retrieved in this way are accessible via `jsl10n` variable:

```html
<script type="text/javascript">
   alert(window.jsl10n.MyProject.AlternativeResources.ThirdProperty);
</script>
```

**NB!** Notice that naming notation of the resource is exactly the same as it's on the server-side. This notation should reduce confusion and make is less problematic to switch from server-side code to front-end, and vice versa.

### Aliases

Sometimes it's required to split resources into different groups and have access to them separately. Also the same problem will occur when you would like to retrieve resources from two different namespaces in single page. Therefore aliasing particular group of resources might come handy. You have to specify `alias` query string parameter:

```html
<script src="/jsl10n/MyProject.MyResources?alias=m"></script>
<script src="/jsl10n/MyProject.AlternativeResources?alias=ar"></script>

<script type="text/javascript">
   alert(window.m.MyProject.MyResources.FirstProperty);

   alert(window.ar.MyProject.AlternativeResources.ForthProperty);
</script>
```

## Explicit Translation Culture

Sometimes it's also necessary to fetch translations for other language. This is possible specifying `lang` query parameter.

```
@Html.GetTranslations(typeof(MyProject), language: "no")
```

or

```html
<script src="/jsl10n/MyProject?lang=no"></script>
```

**Note:** Those resources that do not have translation in requested language will not be emitted in resulting `json` response.
