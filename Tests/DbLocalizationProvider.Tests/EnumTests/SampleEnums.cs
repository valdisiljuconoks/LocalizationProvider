namespace DbLocalizationProvider.Tests.EnumTests
{
    [LocalizedResource]
    public enum SampleStatus
    {
        None = 0,
        New = 1,
        Open = 2
    }

    [LocalizedResource(KeyPrefix = "ThisIsPrefix")]
    public enum SampleStatusWithPrefix
    {
        None = 0,
        New = 1,
        Open = 2
    }

    [LocalizedResource]
    public enum SampleEnumWithKeys
    {
        None = 0,
        [ResourceKey("/this/is/key")] New = 1,
        Open = 2
    }

    [LocalizedResource(KeyPrefix = "/this/is/prefix")]
    public enum SampleEnumWithKeysWithClassPrefix
    {
        None = 0,
        [ResourceKey("/and/this/is/key")] New = 1,
        Open = 2
    }
}
