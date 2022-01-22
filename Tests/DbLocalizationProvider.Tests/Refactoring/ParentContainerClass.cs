using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    public class ParentContainerClass
    {
        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClass
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass", OldNamespace = "In.Galaxy.Far.Far.Away")]
        public class RenamedNestedResourceClassAndNamespace
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        [RenamedResource(OldNamespace = "In.Galaxy.Far.Far.Away")]
        public class RenamedNestedResourceNamespace
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        public class RenamedNestedResourceProperty
        {
            [RenamedResource("OldProperty")]
            public static string NewResourceKey => "New Resource Key";
        }
        
        [LocalizedResource]
        [RenamedResource("OldNestedResourceClassAndProperty")]
        public class RenamedNestedResourceClassAndProperty
        {
            [RenamedResource("OldProperty")]
            public static string NewResourceKey => "New Resource Key";
        }
    }
}
