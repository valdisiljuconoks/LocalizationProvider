using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using DbLocalizationProvider.Tests;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.Tests.ClientsideProvider;

public class RequestHandlerTests
{
    [Fact]
    public void TestSerializationToJson()
    {
        var sut = new RequestHandler(null);
        var context = new ConfigurationContext();
        List<LocalizationResource> resources =
        [
            new("SampleNamespace.SampleResource.Prop1", true)
            {
                Translations =
                {
                    new LocalizationResourceTranslation { Language = "", Value = "p1" },
                    new LocalizationResourceTranslation { Language = "en", Value = "property 1" }
                }
            },
            new("SampleNamespace.SampleResource.Prop2", true)
            {
                Translations =
                {
                    new LocalizationResourceTranslation { Language = "", Value = "p2" },
                    new LocalizationResourceTranslation { Language = "en", Value = "property 2" }
                }
            },
            new("SampleNamespace.SampleResource.Prop3", true)
            {
                Translations =
                {
                    new LocalizationResourceTranslation { Language = "", Value = "p3" },
                    new LocalizationResourceTranslation { Language = "en", Value = "property 3" }
                }
            },
            new("SampleNamespace.SampleResource.Prop4", true)
            {
                Translations =
                {
                    new LocalizationResourceTranslation { Language = "", Value = "p4" },
                    new LocalizationResourceTranslation { Language = "en", Value = "property 4" }
                }
            },
            new("SampleNamespace.SampleResource.Prop5", true)
            {
                Translations =
                {
                    new LocalizationResourceTranslation { Language = "", Value = "p5" },
                    new LocalizationResourceTranslation { Language = "en", Value = "property 5" }
                }
            }
        ];

        context.TypeFactory
            .ForQuery<GetAllResources.Query>()
            .SetHandler(() => new GetAllResourcesUnitTestHandler(resources));

        var options = new OptionsWrapper<ConfigurationContext>(context);

        var json = sut.GetJson("SampleNamespace.SampleResource",
                               "en",
                               false,
                               false,
                               new QueryExecutor(context.TypeFactory),
                               options,
                               new ScanState());

        Assert.Equal(
            @"{""SampleNamespace"":{""SampleResource"":{""Prop1"":""property 1"",""Prop2"":""property 2"",""Prop3"":""property 3"",""Prop4"":""property 4"",""Prop5"":""property 5""}}}",
            json);
    }
}
