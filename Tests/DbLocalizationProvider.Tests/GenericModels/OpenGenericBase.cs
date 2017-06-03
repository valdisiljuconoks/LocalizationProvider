using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Tests.GenericModels
{
    [LocalizedModel]
    public class OpenGenericBase<T> where T : ISampleInterface
    {
        [Include]
        public T BaseProperty { get; set; }
    }
}
