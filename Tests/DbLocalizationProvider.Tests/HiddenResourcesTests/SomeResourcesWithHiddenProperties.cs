namespace DbLocalizationProvider.Tests.HiddenResourcesTests
{
    [LocalizedResource]
    public class SomeResourcesWithHiddenProperties
    {
        [Hidden]
        public static string SomeProperty => "Default value for property";

        public static string AnotherProperty => "Default value for property";
    }

    [LocalizedResource]
    [Hidden]
    public class SomeResourcesWithHiddenOnClassLevel
    {
        public static string SomeProperty => "Default value for property";
    }

    [LocalizedModel]
    public class SomeModelWithHiddenProperty
    {
        [Hidden]
        public string SomeProperty { get; set; }
    }

    [LocalizedModel]
    [Hidden]
    public class SomeModelWithHiddenPropertyOnClassLevel
    {
        public string SomeProperty { get; set; }
    }

    [LocalizedResource]
    public enum SomeEnumWithHiddenResources
    {
        None,
        [Hidden] Some,
        Another
    }

    [LocalizedResource]
    [Hidden]
    public enum SomeEnumWithAllHiddenResources
    {
        None,
        Some,
        Another
    }
}
