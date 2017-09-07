using DbLocalizationProvider.Abstractions.Refactoring;
using Xunit;

namespace DbLocalizationProvider.Tests.Refactoring
{
    [LocalizedResource]
    [RenamedResource("OldResourceClass",
                     OldNamespace = "In.Galaxy.Far.Far.Away")]
    public class NewResourceClass
    {
        [RenamedResource("OldResourceKey")]
        public static string NewResourceKey => "New Resource Key";
    }
}





public class Class1
    {
        [Fact]
        public void Test() { }
    }
}
