using DbLocalizationProvider.EPiServer;
using DbLocalizationProvider.EPiServer.Categories;
using EPiServer.DataAbstraction;

namespace AlloySampleSite.Models.Categories
{
    [LocalizedCategory]
    public sealed class Class1 : Category
    {
        public Class1(Category parent) : base(parent, "Category1")
        {
            Description = "test desc";
        }
    }
}
