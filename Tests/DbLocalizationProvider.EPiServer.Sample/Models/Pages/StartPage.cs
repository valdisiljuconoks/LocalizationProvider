using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace DbLocalizationProvider.EPiServer.Sample.Models.Pages
{
    [ContentType(DisplayName = "StartPage", GUID = "95a3aafc-7f64-4168-a7f6-53cccc1557b3", Description = "")]
    public class StartPage : PageData
    {
        [CultureSpecific]
        [Display(
             Name = "Main body",
             Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
             GroupName = SystemTabNames.Content,
             Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}
