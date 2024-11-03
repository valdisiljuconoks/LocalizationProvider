# Migrations (Renames & Refactorings)

It's been hanging around for a while on GitHub and I never had chance to implemented it properly and I was always thinking about that problem deeper as it should be.. But I finally got question in Amsterdam about this tool - "what if I misspell resource class name, what about it property name that holds a translation is wrongly named at the beginning?" Meaning that if you just rename the resource container class - all the resources underneath will be treated as new set of resources - hence, loosing already added translation (if any). There are also scenarios when developers just want to shuffle around classes and move those in appropriate namespaces. Unfortunately - as database localization provider is heavily based on class structure and metadata found there - renames or class movement around to different namespace is disaster for the localization provider.

There were no support for these kind of refactoring tasks util now.

So it latest version there are bunch of new ways to tackle this complexity of migrations of the resources preserving translations for all added languages.

First, this code snippet might demo you basic support for the refactorings:

```csharp
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedModel]
    [RenamedResource("OldModelClass")]
    public class RenamedModelClass
    {
        public string NewProperty { get; set; }
    }
}
```

When provider will discover this resource newly generated key will be:

 * `DbLocalizationProvider.Abstractions.Refactoring.RenamedModelClass.NewProperty`

However, as resource class is decorated with `[RenamedResource]` attribute, there will be also another resource key discovered:

 * `DbLocalizationProvider.Abstractions.Refactoring.OldModelClass.NewProperty`

Meaning that latter will be used before synchronization. Provider will look for `..OldModelClass.NewProperty` resource in database and will update key to `..RenamedModelClass.NewProperty`.

And only then will proceed with standard resource synchronization.

Various kinds of refactoring scenarios are covered, starting from simple class or resource property name, ending with nested resource class property and class and namespace (for declaring type) renames. This tiny fragment from unit test suite should give you pretty clear picture of supported cases.

```csharp
namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldParentContainerClassAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedParentContainerClassAndNamespace
    {
        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClass
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClassAndProperty
        {
            [RenamedResource("OldResourceKey")]
            public static string NewResourceKey => "New Resource Key";
        }
    }
}
```

This might come handy if you are dealing with requirements to rename resources or models and want to preserve existing translations.

**NB!** Once migration is done and executed successfully, it's recommended that you remove obsolete migrations as they might impact startup performance performing not necessary lookups in database trying to figure out which resource to rename and also because resource class looks ugly with all those attributes and ceremony.
