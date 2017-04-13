# Synchronization Process

## How Sync Works?

Once localization provider will have chance to run during application startup, it will be eager to scan and register all discovered localization resources. 

Sync process will look for either `[LocalizedResource]` or `[LocalizedModel]` annotated types and will start resource discovery from properties of those found types.


All localization resources are split into 2 types: resources and models. 

* **Resources** - localized resources are just list of key / value pairs. You may have a key for the resource and value is its translation in specific language. Resources are designed as just POCO objects.
* **Models** - models are usually view models that may have `DataAnnotation` attributes attached to them (like, `Display`, `Required`, etc).

Read more about resource types [here](resource-types.md).


## Customizing Sync Process

There are couple of attributes you can use to control sync process.

### Ignoring Properties

Adding `[Ignore]` attribute - you are telling sync process to ignore property. This resource will not be discovered duing sync process:

```csharp
using DbLocalizationProvider.Sync;

namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        [Ignore]
        public static string SampleResource => "This is default value";
    }
}
```

### Including Properties

As mentioned sync process supports only scalar / simple date types. Adding `[Include]` attribute - you are telling sync process to add this property to list of discovered resources as well. This resource will be discovered duing sync:

```csharp
using DbLocalizationProvider.Sync;

namespace MySampleProject {

    [LocalizedResource]
    public class MyResources
    {
        [Include]
        public static SomeComplexType SampleResource { get; set };
    }
}
```



## Manually Register Resources

If it's required - you can register resources manually (you are taking care about generated proper resource key as well).
Registration culture is taken from `ConfigurationContext.Current.DefaultResourceCulture` property (default `"en"`):

```csharp
var synchronizer = new ResourceSynchronizer();
synchronizer.RegisterManually(new[] { new ManualResource("This.Is.Sample.Key", translation) });
````

Retrieval:

```csharp
LocalizationProvider.Current.GetString("This.Is.Sample.Key");
```
