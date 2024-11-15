using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource]
    public enum SampleEnum
    {
        [Display(Name = "Nooone")]
        None = 0,
        FirstOption,
        SecondOption
    }
}
