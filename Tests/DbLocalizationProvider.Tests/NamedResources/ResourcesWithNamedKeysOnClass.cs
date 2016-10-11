namespace DbLocalizationProvider.Tests.NamedResources
{
    [LocalizedResource(KeyPrefix = "/this/is/root")]
    public static class ResourcesWithNamedKeysOnClass
    {
        public static string PageHeader => "This is header";
    }
}
