
LocalizedModelAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

LocalizedResourceAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

ResourceKeyAttribute moved from `DbLocalizationProvider` to `DbLocalizationProvider.Abstractions` namespace

Removed `DbLocalizationProvider.LocalizationProvider.Current` static property.

Moved `Translate(this Enum target, params object[] formatArguments)` to `LocalizationProvider.Translate()`

`DbLocalizationProvider.ResourceKeyBuilder` lost all static methods.

`DbLocalizationProvider.ExpressionHelper` lost all static methods.

