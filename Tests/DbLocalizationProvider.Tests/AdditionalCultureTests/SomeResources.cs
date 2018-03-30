using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    [LocalizedResource]
    public class SomeResources
    {
        [TranslationForCulture("Navn", "no")]
        public static string SomeProperty => "Name";
    }

    [LocalizedResource]
    public enum SomeEnumResource
    {
        [TranslationForCulture("Navn", "no")]
        Name
    }

    [LocalizedResource]
    public enum SomeEnumResourceWithDuplicateCultures
    {
        [TranslationForCulture("Navn", "no")]
        [TranslationForCulture("Navn 2", "no")]
        Name
    }

    [LocalizedResource]
    public class SomeResourcesWithDuplicateCultures
    {
        [TranslationForCulture("Navn", "no")]
        [TranslationForCulture("Navn 2", "no")]
        public static string SomeProperty => "Name";

        public static string NormalProperty => "This is normal";
    }
}
