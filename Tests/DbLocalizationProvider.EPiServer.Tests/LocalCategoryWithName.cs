using EPiServer.DataAbstraction;

namespace DbLocalizationProvider.EPiServer.Tests
{
    [LocalizedCategory]
    public class LocalCategoryWithName : Category
    {
        public LocalCategoryWithName()
        {
            Name = "local category";
        }
    }
}
