namespace DbLocalizationProvider.Tests.EnumTests
{
    [LocalizedResource(KeyPrefix = "ThisIsPrefix")]
    public enum SampleStatusWithPrefix
    {
        None = 0,
        New = 1,
        Open = 2
    }
}
