namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    [LocalizedModel]
    public class ModelWithCustomAttributesDuplicates
    {
        [FancyHelpText]
        [FancyHelpText]
        public string UserName { get; set; }
    }
}