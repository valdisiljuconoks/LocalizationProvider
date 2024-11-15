using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource]
    public static class SecretResources
    {
        [Hidden]
        public static string SomeSecretProperty => "THIS IS SECRET!";
    }
}
