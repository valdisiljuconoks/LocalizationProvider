using System.Linq;
using DbLocalizationProvider.Queries;
using Xunit;

namespace DbLocalizationProvider.EPiServer.Tests
{
    //public class CategoriesTests
    //{
    //    public CategoriesTests()
    //    {
    //        ConfigurationContext.Setup(cfg => cfg.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>());
    //    }

    //    [Fact]
    //    public void EpiCateogry_ShouldGetProperResourceKey()
    //    {
    //        var sut = new LocalizedCategoryScanner();
    //        var target = typeof(LocalCategory);

    //        var result = sut.GetClassLevelResources(target, null);

    //        Assert.NotEmpty(result);

    //        var first = result.First();

    //        Assert.Equal($"/categories/category[@name=\"{nameof(LocalCategory)}\"]/description", first.Key);
    //        Assert.Equal("LocalCategory", first.Translations.First().Translation);
    //    }

    //    [Fact]
    //    public void EpiCateogry_ShouldGetProperResourceKey_TranslationFromName()
    //    {
    //        var sut = new LocalizedCategoryScanner();
    //        var target = typeof(LocalCategoryWithName);

    //        var result = sut.GetClassLevelResources(target, null);
    //        var first = result.First();

    //        Assert.Equal($"/categories/category[@name=\"{nameof(LocalCategoryWithName)}\"]/description", first.Key);
    //        Assert.Equal("local category", first.Translations.First().Translation);
    //    }

    //    [Fact]
    //    public void ScanEpiCateogry_ShouldRegister()
    //    {
    //        var sut = new LocalizedCategoryScanner();

    //        var result = sut.ShouldScan(typeof(LocalCategory));

    //        Assert.True(result);
    //    }

    //    [Fact]
    //    public void ScanNonEpiCategory_ShouldNotRegister()
    //    {
    //        var sut = new LocalizedCategoryScanner();

    //        var result = sut.ShouldScan(typeof(LocalNonEpiCategory));

    //        Assert.False(result);
    //    }
    //}
}
