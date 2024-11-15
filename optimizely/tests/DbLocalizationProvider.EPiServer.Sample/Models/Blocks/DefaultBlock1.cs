using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace DbLocalizationProvider.EPiServer.Sample.Models.Blocks
{
    [ContentType(DisplayName = "DefaultBlock1", GUID = "25f4a73c-7e2b-407d-a0e3-03839167524c", Description = "")]
    //[LocalizedResource(Inherited = false)]
    public class DefaultBlock1 : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Name { get; set; }

        [ResourceKey("/something/andthis")]
        [Ignore]
        public string SomePorperty { get; set; }
    }
}
