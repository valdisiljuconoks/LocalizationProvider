
LocalizedModelAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

LocalizedResourceAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

ResourceKeyAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

Removed `DbLocalizationProvider.LocalizationProvider.Current` static property.

Moved `Translate(this Enum target, params object[] formatArguments)` to `LocalizationProvider.Translate()`

`DbLocalizationProvider.ResourceKeyBuilder` lost all static methods.

`DbLocalizationProvider.ExpressionHelper` lost all static methods.


## Data Model

* `LocalizationResource.Translations` from `ICollection<LocalizationResourceTranslation>` to `LocalizationResourceTranslationCollection`
* introduced `IResourceRepository` - for easier storage implementations
* moved `LocalizedResource` & `LocalizedTranslation` to `DbLocalizationProvider.Abstractions` project
* moved `DiscoveredResource` from `DbLocalizationProvider.Sync` to `DbLocalizationProvider.Abstractions`


## Configuration

* Removed

```
ConfigurationContext.Setup()
ConfigurationContext.Current
```


* Converted to method

```
ConfigurationContext.ResourceLookupFilter
```


* Rename

```
ConfigurationContext.FallbackCultures
```

to

```
ConfigurationContext.FallbackLanguages
```

## Sync

* Refactored from static `DiscoveredTranslation.FromSingle` to `DiscoveredTranslationBuilder.FromSingle`
* 

## Azure Functions runtime

```
builder.Services.BuildServiceProvider().UseDbLocalizationProvider();
```
