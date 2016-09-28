namespace DbLocalizationProvider.Tests.InheritedModels
{
    [LocalizedModel]
    public class SampleViewModelWithBase : BaseViewModel
    {
        public string ChildProperty { get; set; }
    }
}