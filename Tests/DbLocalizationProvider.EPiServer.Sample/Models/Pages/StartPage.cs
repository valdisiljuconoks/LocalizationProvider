using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Sync;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiIgnore = EPiServer.DataAnnotations.IgnoreAttribute;

namespace DbLocalizationProvider.EPiServer.Sample.Models.Pages
{
    [ContentType(DisplayName = "StartPage", GUID = "95a3aafc-7f64-4168-a7f6-53cccc1557b3", Description = "")]
    [LocalizedModel(OnlyIncluded = true)]
    public class StartPage : PageData
    {
        public StartPage()
        {
            Address = new AddressViewModel();
        }

        [CultureSpecific]
        [Display(
             Name = "Main body",
             Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
             GroupName = SystemTabNames.Content,
             Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        [EPiIgnore]
        [Include]
        public string SampleOptionalProperty { get; set; }

        [Include]
        [EPiIgnore]
        public AddressViewModel Address { get; set; }

    }

    [LocalizedModel(Inherited = false)]
    public class AddressViewModel : AddressBaseModel
    {
        public string Street { get; set; }
    }

    [LocalizedModel]
    public class AddressBaseModel
    {
        [HelpText]
        public string PostalCodeFromBase { get; set; }
    }
}
