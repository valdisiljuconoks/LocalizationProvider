using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldParentContainerClass")]
    public class RenamedParentContainerClass
    {
        [LocalizedResource]
        [RenamedResource("OldNestedResourceClass")]
        public class RenamedNestedResourceClass
        {
            public static string NewResourceKey => "New Resource Key";
        }

        [LocalizedResource]
        [RenamedResource("OldNestedResourceClassAndProperty")]
        public class RenamedNestedResourceClassAndProperty
        {
            [RenamedResource("OldResourceKey")]
            public static string NewResourceKey => "New Resource Key";
        }
    }
}
