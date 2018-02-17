## What is the LocalizationProvider project?

LocalizationProvider project is Asp.Net Mvc web application localization provider on steriods.

Giving you main following features:
* Database driven localization provider for Asp.Net Mvc applications projects
* Easy resource registrations via code
* Supports hierarchical resource organization (with help of child classes)
* Administration UI for editors to change or add new translations for required languages

## Source Code Repos
Whole package of libraries is split into multiple git repos (with submodule linkage in between). Below is list of all related repositories:
* [Main Repository](https://github.com/valdisiljuconoks/LocalizationProvider/)
* [Asp.Net Core Repository](https://github.com/valdisiljuconoks/localization-provider-core)
* [Asp.Net Repository](https://github.com/valdisiljuconoks/localization-provider-aspnet)
* [Episerver Repository](https://github.com/valdisiljuconoks/localization-provider-epi)

## Getting Started (Asp.Net)
* [Getting Started](https://github.com/valdisiljuconoks/localization-provider-aspnet/blob/master/docs/getting-started-net.md)

## Getting Started (Asp.Net Core)
* [Getting Started](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/getting-started-netcore.md)
* [Localizing App Content](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/localizing-content-netcore.md)

## Working with DbLocalizationProvider
* [Localized Resource Types](docs/resource-types.md)
* [Synchronization Process](docs/sync-net.md)
* [Working with Resources](docs/working-with-resources-net.md)
* [Translating System.Enum Types](docs/translate-enum-net.md)
* [Mark Required Fields](docs/required-fields.md)
* [Foreign Resources](docs/foreign-resources.md)
* [Hidden Resources](docs/hidden-resources.md)
* [Reference Other Resource](docs/ref-resources.md)
* [Cache Event Notifications](docs/cache-events.md)
* [XLIFF Support](docs/xliff.md)
* [Migrations & Refactorings](docs/migr.md)

## Integrating with EPiServer
* For more information about Episerver integration - read [here](https://github.com/valdisiljuconoks/localization-provider-epi/blob/master/README.md)

## Build Statuses

|    | Main | Asp.Net | .Net Core | Episerver |
|:---|-----:|--------:|----------:|----------:|
|**VSTS Build**|[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/a3f0ad74-99ed-446b-8cb9-ff35e99a6e2b/12/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider/_build/index?definitionId=12)|[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/70e95aed-5f16-4125-b7bb-60aeea07539d/10/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider-aspnet/_build/index?definitionId=10)|[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/f63fd8ab-e3f1-48c1-bca0-f027727a53c4/9/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider-core/_build/index?definitionId=9)|[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/7cf5a00f-7a74-440c-83bd-45d6c8a80602/11/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider-epi/_build/index?definitionId=11)|

# More Info

* [Part 1: Resources and Models](http://blog.tech-fellow.net/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/)
* [Part 3: Import and Export](http://blog.tech-fellow.net/2017/02/22/localization-provider-import-and-export-merge/)
* [Part 4: Resource Refactoring and Migrations](https://blog.tech-fellow.net/2017/10/10/localizationprovider-tree-view-export-and-migrations/)
