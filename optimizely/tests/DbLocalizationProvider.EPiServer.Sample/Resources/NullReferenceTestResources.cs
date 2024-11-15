namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource]
    public class NullReferenceTestResources
    {
        public static NestedResourceClass StaticNested { get; }

        public class NestedResourceClass
        {
            public string SomeRes => "Some res translation";
        }

        public InstanceNestedResourceClass InstanceNested { get; }

        public class InstanceNestedResourceClass
        {
            public string SomeRes => "Some instance res translation";
        }

    }
}
