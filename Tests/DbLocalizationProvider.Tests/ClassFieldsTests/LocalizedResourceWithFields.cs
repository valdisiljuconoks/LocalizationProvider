using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.ClassFieldsTests
{
    [LocalizedResource]
    public class LocalizedResourceWithFields
    {
        public static string ThisisField = "sample value";
    }

    [LocalizedResource]
    public class LocalizedResourceWithIgnoredFields
    {
        [Ignore]
        public static string ThisisAnotherField = "another value";
    }
}
