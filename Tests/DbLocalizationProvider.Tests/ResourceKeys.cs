using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests
{
    [LocalizedResource]
    public class PageResources
    {
        public static HeaderResources Header { get; set; }

        public class HeaderResources
        {
            public string HelloMessage => "This is default value for resource";
        }
    }

    [LocalizedResource]
    public class CommonResources
    {
        [LocalizedResource]
        public class DialogResources
        {
            public static string YesButton { get; set; }

            public static string NullProperty = null;
        }
    }

    public class ParentClassForResources
    {
        [LocalizedResource]
        public class ChildResourceClass
        {
            public static string HelloMessage => "This is default value for resource";
        }
    }

    [LocalizedResource]
    public class ResourceKeys
    {
        public ResourceKeys()
        {
            SubResource = new SubResourceKeys();
        }

        public string SampleResource { get; set; }

        public static SubResourceKeys SubResource { get; set; }

        public static string ThisIsConstant => "Default value for constant";
    }

    [LocalizedResource]
    public class SubResourceKeys
    {
        public SubResourceKeys()
        {
            EvenMoreComplexResource = new DeeperSubResourceModel();
        }

        public string SubResourceProperty => "Sub Resource Property";

        public int AnotherResource { get; set; }

        public DeeperSubResourceModel EvenMoreComplexResource { get; set; }
    }

    [LocalizedResource]
    public class DeeperSubResourceModel
    {
        public decimal Amount { get; set; }
    }
}
