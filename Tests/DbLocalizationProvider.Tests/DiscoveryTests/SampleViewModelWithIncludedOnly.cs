using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    [LocalizedModel(OnlyIncluded = true)]
    public class SampleViewModelWithIncludedOnly
    {
        [Include]
        public string IncludedProperty { get; set; }

        public string ExcludedProperty { get; set; }
    }

    public class BaseModel
    {
        public string ExcludedBaseProperty { get; set; }

        [Include]
        public string IncludedBaseProperty { get; set; }
    }

    [LocalizedModel(OnlyIncluded = true)]
    public class SampleViewModelWithIncludedOnlyWithBase : BaseModel
    {
        [Include]
        public string IncludedProperty { get; set; }

        public string ExcludedProperty { get; set; }
    }
}
