using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.Tests;

public class ConfigurationContextTests
{
    [Fact]
    public void FallbackLanguagesCollectionTest()
    {
        var sut = new ServiceCollection();

        sut.AddDbLocalizationProvider(ctx =>
        {
            ctx.FallbackLanguages.Try(new CultureInfo("lv"));
        });

        var f = sut.FirstOrDefault(s => s.ServiceType.IsAssignableFrom(typeof(IConfigureOptions<ConfigurationContext>)));

        Assert.NotNull(f);

        var sp = sut.BuildServiceProvider();

        var ctx = sp.GetRequiredService<IOptions<ConfigurationContext>>().Value;
        Assert.Equal(1, ctx.FallbackLanguages.Count);
        Assert.Equal(1, ctx._fallbackCollection.GetFallbackLanguages("default").Count);
    }
}
