using BenchmarkDotNet.Attributes;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AspNetCore.ClientsideProvider;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using DbLocalizationProvider.Tests;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.Tests.ClientsideProvider;

[MemoryDiagnoser]
public class RequestHandlerBenchmarkTests
{
    private ConfigurationContext _context;
    private OptionsWrapper<ConfigurationContext> _options;
    private QueryExecutor _queryExecutor;
    private readonly ScanState _scanState = new();
    private RequestHandler _sut;

    [GlobalSetup]
    public void Setup()
    {
        _sut = new RequestHandler(null);
        _context = new ConfigurationContext();
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

        _context.TypeFactory
            .ForQuery<GetAllResources.Query>()
            .SetHandler(() => new GetAllResourcesUnitTestHandler(resources));

        _options = new OptionsWrapper<ConfigurationContext>(_context);

        _queryExecutor = new QueryExecutor(_context.TypeFactory);
    }

    [Benchmark]
    public void ConvertWithInlineJsonSettings()
    {
            _sut.GetJson("SampleNamespace.SampleResource",
                         "en",
                         false,
                         false,
                         _queryExecutor,
                         _options,
                         _scanState);

            _sut.GetJson("SampleNamespace.SampleResource",
                         "en",
                         true,
                         false,
                         _queryExecutor,
                         _options,
                         _scanState);

            _sut.GetJson("SampleNamespace.SampleResource",
                         "en",
                         false,
                         true,
                         _queryExecutor,
                         _options,
                         _scanState);

            _sut.GetJson("SampleNamespace.SampleResource",
                         "en",
                         true,
                         true,
                         _queryExecutor,
                         _options,
                         _scanState);
    }
}
