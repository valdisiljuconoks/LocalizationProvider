[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=valdisiljuconoks_LocalizationProvider&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=valdisiljuconoks_LocalizationProvider)

# Supporting LocalizationProvider

If you find this library useful, cup of coffee would be awesome! You can support further development of the library via [Paypal](https://paypal.me/valdisiljuconoks).

# Localization Provider v8.0 IS OUT!

I'm pleased to announce that Localization Provider v8.0 is finally out. Again - took a bit longer than expected :)

What's new?

* .NET8 set as default target
* Added provider model for translations - starting with [Azure AI](docs/translators.md) for automatic translations
* `ConfigurationContext` now supports config configuration as well (you can change some settings after you have added and configured default settings for localization provider). This is very useful in unit test scenarios when you need to adjust some settings for specific test.
* Various bug fixes
* Some performance improvements (resource key comparison, [pagination in Admin UI](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/getting-started-adminui.md))
* Security improvements (by default upgrading insecure connections)
* Dependencies upgrade

More info in this [blog post](https://tech-fellow.eu/2024/02/28/localization-provider-v8-released/).

# What is the LocalizationProvider project?

LocalizationProvider project is ASP.NET Mvc web application localization provider on steroids.

Giving you the main following features:
* Database-driven localization provider for Asp.Net Mvc applications projects
* Easy resource registrations via code
* Supports hierarchical resource organization (with help of child classes)
* Administration UI for editors to change or add new translations for required languages

## Source Code Repos
The whole package of libraries is split into multiple git repos (with submodule linkage in between). Below is list of all related repositories:
* [Main Repository](https://github.com/valdisiljuconoks/LocalizationProvider/)
* [.NET Runtime Repository](https://github.com/valdisiljuconoks/localization-provider-core)
* [Optimizely Integration Repository](https://github.com/valdisiljuconoks/localization-provider-epi)

## Getting Started (.NET)
* [Getting Started](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/getting-started-netcore.md)
* [Getting Started with AdminUI](https://github.com/valdisiljuconoks/localization-provider-core/blob/master/docs/getting-started-adminui.md)

## Working with DbLocalizationProvider Package
* [Localized Resource Types](docs/resource-types.md)
* [Synchronization Process](docs/sync-net.md)
* [MSSQL Storage Configuration](docs/mssql.md)
* [Working with Resources](docs/working-with-resources-net.md)
* [Working with Languages](docs/working-with-languages-net.md)
* [Translating System.Enum Types](docs/translate-enum-net.md)
* [Mark Required Fields](docs/required-fields.md)
* [Foreign Resources](docs/foreign-resources.md)
* [Hidden Resources](docs/hidden-resources.md)
* [Reference Other Resource](docs/ref-resources.md)
* [Cache Event Notifications](docs/cache-events.md)
* [XLIFF Support](docs/xliff.md)
* [Migrations & Refactorings](docs/migr.md)

## Integrating with Optimizely
* For more information about Optimizely integration - read [here](https://github.com/valdisiljuconoks/localization-provider-epi/blob/master/README.md)

# Other Versions

## Localization Provider v7.x Released

Please read more in [this blog post](https://tech-fellow.eu/2022/01/23/dblocalizationprovider-for-optimizely/)!

## Upgrade to v7.x?

Please read more details in [this blog post](https://tech-fellow.eu/2022/01/23/dblocalizationprovider-for-optimizely/)!

## What was new in v6.x?

Please [refer to this post](https://tech-fellow.eu/2020/02/22/localization-provider-major-6/) to read more about new features in v6.


# More Info

* [Part 1: Resources and Models](https://tech-fellow.eu/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](https://tech-fellow.eu/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/)
* [Part 3: Import and Export](https://tech-fellow.eu/2017/02/22/localization-provider-import-and-export-merge/)
* [Part 4: Resource Refactoring and Migrations](https://tech-fellow.eu/2017/10/10/localizationprovider-tree-view-export-and-migrations/)
