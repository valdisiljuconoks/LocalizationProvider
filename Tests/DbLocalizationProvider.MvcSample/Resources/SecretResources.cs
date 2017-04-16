using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.MvcSample.Resources
{
    [LocalizedResource]
    public static class SecretResources
    {
        [Hidden]
        public static string SomeSecretProperty => "THIS IS SECRET!";
    }
}
