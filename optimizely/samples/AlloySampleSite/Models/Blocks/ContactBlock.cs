using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace AlloySampleSite.Models.Blocks
{
    /// <summary>
    /// Used to present contact information with a call-to-action link
    /// </summary>
    /// <remarks>Actual contact details are retrieved from a contact page specified using the ContactPageLink property</remarks>
    [SiteContentType(GUID = "7E932EAF-6BC2-4753-902A-8670EDC5F363")]
    [SiteImageUrl]
    public class ContactBlock : SiteBlockData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Image { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [CultureSpecific]
        public virtual string Heading { get; set; }

        /// <summary>
        /// Gets or sets the contact page from which contact information should be retrieved
        /// </summary>
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 3)]
        [UIHint(Global.SiteUIHints.Contact)]
        public virtual PageReference ContactPageLink { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 4)]
        [CultureSpecific]
        public virtual string LinkText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 5)]
        [CultureSpecific]
        public virtual Url LinkUrl { get; set; }
    }
}