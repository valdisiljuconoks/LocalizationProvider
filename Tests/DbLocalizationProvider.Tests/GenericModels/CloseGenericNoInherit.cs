using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.GenericModels
{
    [LocalizedModel(Inherited = false)]
    public class CloseGenericNoInherit : OpenGenericBase<SampleImpl>
    {
        public string ChildProperty { get; set; }
    }
}
