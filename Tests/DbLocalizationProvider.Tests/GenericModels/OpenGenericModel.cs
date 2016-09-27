using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Tests.GenericModels
{
    [LocalizedModel]
    public class OpenGenericModel<T> where T : ISampleInterface
    {
        [Include]
        public T GenericProperty { get; set; }
    }
}
