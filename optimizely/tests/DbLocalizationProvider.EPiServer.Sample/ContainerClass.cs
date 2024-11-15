using DbLocalizationProvider;

namespace MyProject
{
    public class ContainerClass
    {
        public class NestedClass
        {
            [LocalizedResource]
            public class TheResourceClass
            {
                public static string SomeKey => "This is resource";
            }
        }
    }
}
