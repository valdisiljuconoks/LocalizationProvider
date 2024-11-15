using AlloySampleSite.Models.Blocks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors.SelectionFactories;
using EPiServer.Shell.ObjectEditing;

namespace AlloySampleSite.Models.Pages
{
    /// <summary>
    /// Used for the site's start page and also acts as a container for site settings
    /// </summary>
    [ContentType(
        GUID = "19671657-B684-4D95-A61F-8DD4FE60D559",
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Include = new[] { typeof(ContainerPage), typeof(ProductPage), typeof(StandardPage), typeof(ISearchPage), typeof(LandingPage), typeof(ContentFolder) }, // Pages we can create under the start page...
        ExcludeOn = new[] { typeof(ContainerPage), typeof(ProductPage), typeof(StandardPage), typeof(ISearchPage), typeof(LandingPage) })] // ...and underneath those we can't create additional start pages
    [LocalizedResource(KeyPrefix = "/ContentTypes/StartPage/")]
    public class StartPage : SitePageData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        [CultureSpecific]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 300)]
        public virtual LinkItemCollection ProductPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 350)]
        public virtual LinkItemCollection CompanyInformationPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 400)]
        public virtual LinkItemCollection NewsPageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings, Order = 450)]
        public virtual LinkItemCollection CustomerZonePageLinks { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        [ResourceKey("properties/GlobalNewsPageLink/Help", "Some help text for GlobalNewsPageLink")]
        [ResourceKey("properties/GlobalNewsPageLink/Caption", "Caption of the property for GlobalNewsPageLink")]
        public virtual PageReference GlobalNewsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference ContactsPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual PageReference SearchPageLink { get; set; }

        [Display(GroupName = Global.GroupNames.SiteSettings)]
        public virtual SiteLogotypeBlock SiteLogotype { get; set; }

        //[SelectOne(SelectionFactoryType = typeof(LocalizedEnumSelectionFactory<SomeValuesEnum>))]
        [LocalizedSelectOneEnum(typeof(SomeValuesEnum))]
        public virtual SomeValuesEnum SomeValue { get; set; }

        //[SelectMany(SelectionFactoryType = typeof(LocalizedEnumSelectionFactory<SomeValuesEnum>))]
        [LocalizedSelectManyEnum(typeof(SomeValuesEnum))]
        public virtual string SomeMultipleValue { get; set; }

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
        ThirdOne = 3
    }
}
