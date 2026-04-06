using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Optimizely.Sandbox.Business.Rendering;
using EPiServer.Web;

namespace DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;

/// <summary>
/// Represents contact details for a contact person
/// </summary>
[SiteContentType(
    GUID = "F8D47655-7B50-4319-8646-3369BA9AF05B",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl(Globals.StaticGraphicsFolderPath + "page-type-thumbnail-contact.png")]
public class ContactPage : SitePageData, IContainerPage
{
    [Display(GroupName = Globals.GroupNames.Contact)]
    [UIHint(UIHint.Image)]
    public virtual ContentReference Image { get; set; }

    [Display(GroupName = Globals.GroupNames.Contact)]
    public virtual string Phone { get; set; }

    [Display(GroupName = Globals.GroupNames.Contact)]
    [EmailAddress]
    public virtual string Email { get; set; }
}
