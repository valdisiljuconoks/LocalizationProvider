namespace DbLocalizationProvider.Tests.AdditionalCultureTests {
    [LocalizedResource]
    public class SomeResources
    {
        [TranslationForCulture("Navn", "no")]
        public static string SomeProperty => "Name";
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