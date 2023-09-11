using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations;

public class ResourceKeyOnModelTests
{
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly TypeDiscoveryHelper _sut;

    public ResourceKeyOnModelTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        _keyBuilder = new ResourceKeyBuilder(state, ctx);
        var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(_keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         ctx,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder)
                                       },
                                       ctx);
    }

    [Fact]
    public void ModelWithResourceKeysOnValidationAttributes_GetsCorrectCustomKey()
    {
        var container = typeof(ModelWithDataAnnotationsAndResourceKey);
        var properties = _sut.ScanResources(container);

        Assert.NotEmpty(properties);
        Assert.Equal(2, properties.Count());
        Assert.NotNull(properties.First(r => r.Key == "the-key"));

        var requiredResource = properties.First(r => r.Key == "the-key-Required");
        Assert.NotNull(requiredResource);
        Assert.Equal("User name is required!", requiredResource.Translations.DefaultTranslation());

        var resourceKey = _keyBuilder.BuildResourceKey(container, nameof(ModelWithDataAnnotationsAndResourceKey.UserName));
        Assert.Equal("the-key", resourceKey);

        var requiredResourceKey =
            _keyBuilder.BuildResourceKey(container,
                                         nameof(ModelWithDataAnnotationsAndResourceKey.UserName),
                                         new RequiredAttribute());
        Assert.Equal("the-key-Required", requiredResourceKey);
    }

    [Fact]
    public void MultipleAttributesForSingleProperty_WithValidationAttribute()
    {
        var model = _sut
            .GetTypesWithAttribute<LocalizedResourceAttribute>()
            .Where(t => t.FullName
                        == $"DbLocalizationProvider.Tests.DataAnnotations.{nameof(ResourcesWithNamedKeysAndValidationAttributeWithPrefix)}");

        var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

        Assert.Equal(5, properties.Count);
        Assert.NotNull(properties.Single(_ => _.Key == "/root/name"));
        Assert.NotNull(properties.Single(_ => _.Key == "/root/and/this/is/header"));
        Assert.NotNull(properties.Single(_ => _.Key == "/root/and/this/is/another/header"));
        Assert.NotNull(properties.Single(_ => _.Key == "/root/this/is/email"));
        Assert.NotNull(properties.Single(_ => _.Key == "this/is/email-EmailAddress"));
    }
}

[LocalizedModel]
public class ModelWithDataAnnotationsAndResourceKey
{
    [ResourceKey("the-key")]
    [Display(Name = "Something")]
    [Required(ErrorMessage = "User name is required!")]
    public string UserName { get; set; }
}

[LocalizedModel]
public class ModelWithDataAnnotationsAndManyResourceKeys
{
    [ResourceKey("the-key")]
    [ResourceKey("the-key2")]
    [Required]
    public string UserName { get; set; }
}

[LocalizedResource(KeyPrefix = "/root/")]
[ResourceKey("name", Value = "ResourceClass")]
public static class ResourcesWithNamedKeysAndValidationAttributeWithPrefix
{
    [ResourceKey("and/this/is/header", Value = "Header")]
    [ResourceKey("and/this/is/another/header", Value = "Another Header")]
    [Phone]
    public static string PageHeader { get; set; }

    [ResourceKey("this/is/email")]
    [EmailAddress]
    public static string Email { get; set; }
}
