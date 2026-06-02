// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.NotesResourcesTests;

[LocalizedModel]
public class SomeModelWithNotes
{
    [Notes("documents the placeholder {0} in this resource")]
    public string DocumentedProperty { get; set; }

    public string PlainProperty { get; set; }
}

public class NotesDiscoveryTests
{
    private readonly TypeDiscoveryHelper _sut;

    public NotesDiscoveryTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, wrapper, translationBuilder),
                                           new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, wrapper, translationBuilder),
                                           new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, wrapper, translationBuilder)
                                       },
                                       wrapper);
    }

    [Fact]
    public void NotesAttributeOnProperty_FlowsToDiscoveredResource()
    {
        var result = _sut.ScanResources(typeof(SomeModelWithNotes));

        var documented = result.First(r => r.Key.EndsWith(".DocumentedProperty"));
        Assert.Equal("documents the placeholder {0} in this resource", documented.Notes);
    }

    [Fact]
    public void PropertyWithoutNotesAttribute_HasNullNotes()
    {
        var result = _sut.ScanResources(typeof(SomeModelWithNotes));

        var plain = result.First(r => r.Key.EndsWith(".PlainProperty"));
        Assert.Null(plain.Notes);
    }
}
