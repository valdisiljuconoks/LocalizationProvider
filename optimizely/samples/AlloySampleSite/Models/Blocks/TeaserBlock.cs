using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace AlloySampleSite.Models.Blocks
{
    /// <summary>
    /// Used to provide a stylized entry point to a page on the site
    /// </summary>
    [SiteContentType(GUID = "EB67A99A-E239-41B8-9C59-20EAA5936047")] // BEST PRACTICE TIP: Always assign a GUID explicitly when creating a new block type
    [SiteImageUrl] // Use site's default thumbnail
    public class TeaserBlock : SiteBlockData
    {
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [UIHint(UIHint.Textarea)]
        public virtual string Text { get; set; }

        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [UIHint(UIHint.Image)]
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual ContentReference Image { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual PageReference Link { get; set; }
    }
}
