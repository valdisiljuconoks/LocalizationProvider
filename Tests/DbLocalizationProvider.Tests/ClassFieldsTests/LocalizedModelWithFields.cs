using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Tests.ClassFieldsTests
{
    [LocalizedModel]
    public class LocalizedModelWithFields
    {
        public static string AnotherField = "other value";
    }

    [LocalizedModel]
    public class LocalizedModelWithOnlyIgnoredFields
    {
        [Ignore] public static string YetAnotherField = "yet other value";
    }

    [LocalizedModel(OnlyIncluded = true)]
    public class LocalizedModelWithOnlyIncludedFields
    {
        [Include] public static string YetAnotherField = "yet other value";
    }

    [LocalizedModel]
    public class LocalizedBaseModelWithFields
    {
        public static string ThisIsBaseField = "base value";
    }

    [LocalizedModel(Inherited = false)]
    public class LocalizedChildModelWithFields : LocalizedBaseModelWithFields { }
}
