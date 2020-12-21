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



## Register Manually Resources
There is also an option in library to register resources manually.
If you need to register resources manually - you can choose either to do it:
a) during initialization pipeline (Startup.cs, initialization modules, custom startup code, etc.).
b) whenever you need to do it later at some point

### During Initialization Pipelines
If you are registering resources manually during application initialization phase, you have to specificy manual resource provider implementation:

```csharp
ConfigurationContext.Setup(_ =>
{
    // manually register some resources
    _.ManualResourceProvider = new SomeManualResources();

    // rest of the configuration setup
    ...
});

public class SomeManualResources : IManualResourceProvider
{
    public IEnumerable<ManualResource> GetResources()
    {
        return new List<ManualResource>
        {
            new ManualResource("Manual.Resource.1", "Invariant", CultureInfo.InvariantCulture)
        };
    }
}
```

### Later
If you are registering resources later in the pipeline (for example, during Http request), you have to use `ISynchronizer` to do this.

```csharp
public class HomeController : Controller
{
    private readonly ISynchronizer _synchronizer;

    public HomeController(ISynchronizer synchronizer)
    {
        _synchronizer = synchronizer;
    }

    public IActionResult Index()
    {
        // register manually some of the resources
        _synchronizer.RegisterManually(new List<ManualResource>
        {
            new ManualResource("Manual.Resource.1", "English translation", new CultureInfo("en"))
        });
    }
}
```
