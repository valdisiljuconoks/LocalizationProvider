using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.ResourcesAndInheritance {
    [LocalizedResource(Inherited = false)]
    public class ResourceWithBaseClassNoInheritance : BaseResourceClass
    {
        public string PropertyOnResourceClass { get; set; }
    }
}