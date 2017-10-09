using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.Tests.Refactoring {
    [RenamedResource("OldResourceClassAndKeyAndNamespaceWithAdditionalAttribute", OldNamespace = "In.Galaxy.Far.Far.Away")]
    [LocalizedResource]
    public class RenamedResourceClassAndKeyAndNamespaceWithAdditionalAttribute
    {
        [RenamedResource("OldResourceKey")]
        [AdditionalData]
        public static string NewResourceKey => "New Resource Key";
    }
}