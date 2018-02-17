namespace DbLocalizationProvider.Tests.ResourcesAndInheritance {
    [LocalizedResource]
    public class ResourceWithBaseClass : BaseResourceClass
    {
        public string PropertyOnResourceClass { get; set; }
    }
}