using DbLocalizationProvider.Abstractions;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace DbLocalizationProvider.EPiServer.Sample.Models.Pages
{
    [LocalizedResource(KeyPrefix = "/contenttypes/aboutuspage/", Inherited = false)]
    public class SomeWeirdPage : PageData
    {
        [Abstractions.Ignore]
        public virtual ContentArea ContentArea { get; set; }

        [AllowedTypes(typeof(StartPage))]
        [ResourceKey("key1", Value = "1")]
        [ResourceKey("key2", Value = "2")]
        public virtual ContentArea AnotherContentArea { get; set; }

        [Include]
        public virtual string SomeProperty { get; set; }

        [ResourceKey("properties/herotitle/caption", Value = "Hero Title")]
        [ResourceKey("properties/herotitle/help", Value =
            "Set hero title, if nothings is set default one will be used.")]
        public virtual string HeroTitle { get; set; }
    }
}
