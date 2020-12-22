using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldParentContainerClassAndNamespace", OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class RenamedParentContainerClassAndNamespace
    {
        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClass
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClassAndProperty
        {
            [RenamedResource("OldResourceKey")]
            public static string NewResourceKey => "New Resource Key";
        }
    }
}
