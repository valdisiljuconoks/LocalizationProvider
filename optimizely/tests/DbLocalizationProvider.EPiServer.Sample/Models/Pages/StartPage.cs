using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiIgnore = EPiServer.DataAnnotations.IgnoreAttribute;

namespace DbLocalizationProvider.EPiServer.Sample.Models.Pages
{
    [LocalizedModel(KeyPrefix = "/contenttypes/pagedatabase")]
    public class AnotherPage : PageData
    {

    }

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

        [CultureSpecific]
        [Display(
             Name = "Main content area",
             GroupName = SystemTabNames.Content,
             Order = 2)]
        public virtual ContentArea MainContentArea { get; set; }

        [EPiIgnore]
        [Include]
        public string SampleOptionalProperty { get; set; }

        [Include]
        [EPiIgnore]
        public AddressViewModel Address { get; set; }

        [LocalizedEnum(typeof(SomeValuesEnum))]
        [BackingType(typeof(PropertyNumber))]
        public virtual SomeValuesEnum SomeValue { get; set; }

        [LocalizedEnum(typeof(SomeValuesEnum), true)]
        [BackingType(typeof(PropertyNumber))]
        public virtual SomeValuesEnum SomeValueMany { get; set; }

        [SelectOne(SelectionFactoryType = typeof(SampleSelectionFactory))]
        public virtual string PickOne { get; set; }

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

    [LocalizedResource]
    public enum SomeValuesEnum
    {
        [Display(Name = "NOONE!")]
        None = 0,

        [Display(Name = "1st value")]
        FirstValue = 1,

        [Display(Name = "This is second")]
        SecondValue = 2,

        [Display(Name = "And here comes last (3rd)")]
        [TranslationForCulture("Tredje", "sv")]
        [TranslationForCulture("Third (EN)", "en")]
        ThirdOne = 3
    }
}
