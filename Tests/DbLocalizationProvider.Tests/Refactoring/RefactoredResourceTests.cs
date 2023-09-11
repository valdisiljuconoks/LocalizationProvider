using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.Refactoring;

public class RefactoredResourceTests
{
    private readonly TypeDiscoveryHelper _sut;

    public RefactoredResourceTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var keyBuilder = new ResourceKeyBuilder(state, ctx);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        ctx.CustomAttributes.Add<AdditionalDataAttribute>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         ctx,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder)
                                       },
                                       ctx);
    }

    [Theory]
    [InlineData(typeof(RenamedResourceClass),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClass.NewResourceKey",
                "OldResourceClass",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClass.NewResourceKey")]
    [InlineData(typeof(RenamedResourceNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceNamespace.NewResourceKey",
                null,
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.RenamedResourceNamespace.NewResourceKey")]
    [InlineData(typeof(RenamedResourceClassAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndNamespace.NewResourceKey",
                "OldResourceClassAndNamespace",
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.OldResourceClassAndNamespace.NewResourceKey")]
    [InlineData(typeof(RenamedResourceKey),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKey.NewResourceKey",
                "OldResourceKey",
                null,
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKey.OldResourceKey")]
    [InlineData(typeof(RenamedResourceClassAndKey),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKey.NewResourceKey",
                "OldResourceKey",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClass.OldResourceKey")]
    [InlineData(typeof(RenamedResourceClassAndKeyAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKeyAndNamespace.NewResourceKey",
                "OldResourceKey",
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.OldResourceClassAndKeyAndNamespace.OldResourceKey")]
    public void ProperRenameDiscoveryTests(
        Type target,
        string resourceKey,
        string oldTypeName,
        string typeOldNamespace,
        string oldResourceKey)
    {
        var result = _sut.ScanResources(target);

        Assert.NotEmpty(result);
        var discoveredResource = result.First();

        Assert.Equal(resourceKey, discoveredResource.Key);
        Assert.Equal(oldTypeName, discoveredResource.TypeOldName);
        Assert.Equal(typeOldNamespace, discoveredResource.TypeOldNamespace);
        Assert.Equal(oldResourceKey, discoveredResource.OldResourceKey);
    }

    [Theory]
    [InlineData(typeof(ParentContainerClass.RenamedNestedResourceClass),
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceClass.NewResourceKey",
                "OldNestedResourceClass",
                null,
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+OldNestedResourceClass.NewResourceKey")]
    [InlineData(typeof(ParentContainerClass.RenamedNestedResourceProperty),
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceProperty.NewResourceKey",
                "OldProperty",
                null,
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceProperty.OldProperty")]
    [InlineData(typeof(ParentContainerClass.RenamedNestedResourceClassAndProperty),
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceClassAndProperty.NewResourceKey",
                "OldProperty",
                null,
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+OldNestedResourceClassAndProperty.OldProperty")]
    [InlineData(typeof(ParentContainerClass.RenamedNestedResourceClassAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceClassAndNamespace.NewResourceKey",
                "OldNestedResourceClass",
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.ParentContainerClass+OldNestedResourceClass.NewResourceKey")]
    [InlineData(typeof(ParentContainerClass.RenamedNestedResourceNamespace),
                "DbLocalizationProvider.Tests.Refactoring.ParentContainerClass+RenamedNestedResourceNamespace.NewResourceKey",
                null,
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.ParentContainerClass+RenamedNestedResourceNamespace.NewResourceKey")]
    [InlineData(typeof(RenamedParentContainerClass.RenamedNestedResourceClass),
                "DbLocalizationProvider.Tests.Refactoring.RenamedParentContainerClass+RenamedNestedResourceClass.NewResourceKey",
                "OldNestedResourceClass",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldParentContainerClass+OldNestedResourceClass.NewResourceKey")]
    [InlineData(typeof(RenamedParentContainerClass.RenamedNestedResourceClassAndProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedParentContainerClass+RenamedNestedResourceClassAndProperty.NewResourceKey",
                "OldResourceKey",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldParentContainerClass+OldNestedResourceClassAndProperty.OldResourceKey")]
    [InlineData(typeof(RenamedParentContainerClassAndNamespace.RenamedNestedResourceClass),
                "DbLocalizationProvider.Tests.Refactoring.RenamedParentContainerClassAndNamespace+RenamedNestedResourceClass.NewResourceKey",
                "OldNestedResourceClass",
                null,
                "In.Galaxy.Far.Far.Away.OldParentContainerClassAndNamespace+OldNestedResourceClass.NewResourceKey")]
    [InlineData(typeof(RenamedParentContainerClassAndNamespace.RenamedNestedResourceClassAndProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedParentContainerClassAndNamespace+RenamedNestedResourceClassAndProperty.NewResourceKey",
                "OldResourceKey",
                null,
                "In.Galaxy.Far.Far.Away.OldParentContainerClassAndNamespace+OldNestedResourceClass.OldResourceKey")]
    public void NestedResourceProperRenameDiscoveryTests(
        Type target,
        string resourceKey,
        string oldTypeName,
        string typeOldNamespace,
        string oldResourceKey)
    {
        var result = _sut.ScanResources(target);

        Assert.NotEmpty(result);
        var discoveredResource = result.First();

        Assert.Equal(resourceKey, discoveredResource.Key);
        Assert.Equal(oldTypeName, discoveredResource.TypeOldName);
        Assert.Equal(typeOldNamespace, discoveredResource.TypeOldNamespace);
        Assert.Equal(oldResourceKey, discoveredResource.OldResourceKey);
    }

    [Theory]
    [InlineData(typeof(RenamedModelClass),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClass.NewProperty",
                "OldModelClass",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldModelClass.NewProperty")]
    [InlineData(typeof(RenamedModelClassAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassAndNamespace.NewProperty",
                "OldModelClassAndNamespace",
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.OldModelClassAndNamespace.NewProperty")]
    [InlineData(typeof(RenamedModelNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelNamespace.NewProperty",
                null,
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.RenamedModelNamespace.NewProperty")]
    [InlineData(typeof(RenamedModelProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelProperty.NewProperty",
                "OldProperty",
                null,
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelProperty.OldProperty")]
    [InlineData(typeof(RenamedModelClassAndProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassAndProperty.NewProperty",
                "OldProperty",
                null,
                "DbLocalizationProvider.Tests.Refactoring.OldModelClassAndProperty.OldProperty")]
    [InlineData(typeof(RenamedModelClassAndNamespaceAndProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassAndNamespaceAndProperty.NewProperty",
                "OldProperty",
                "In.Galaxy.Far.Far.Away",
                "In.Galaxy.Far.Far.Away.OldModelClassAndNamespaceAndProperty.OldProperty")]
    public void ModelProperRenameDiscoveryTests(
        Type target,
        string resourceKey,
        string oldTypeName,
        string typeOldNamespace,
        string oldResourceKey)
    {
        var result = _sut.ScanResources(target);

        Assert.NotEmpty(result);
        var discoveredResource = result.First();

        Assert.Equal(resourceKey, discoveredResource.Key);
        Assert.Equal(oldTypeName, discoveredResource.TypeOldName);
        Assert.Equal(typeOldNamespace, discoveredResource.TypeOldNamespace);
        Assert.Equal(oldResourceKey, discoveredResource.OldResourceKey);
    }

    [Theory]
    [InlineData(typeof(RenamedModelWithValidationClass),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationClass.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationClass.NewProperty-Required",
                "DbLocalizationProvider.Tests.Refactoring.OldModelWithValidationClass.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.OldModelWithValidationClass.NewProperty-Required")]
    [InlineData(typeof(RenamedModelWithDisplayProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDisplayProperty.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDisplayProperty.NewProperty-Description",
                "DbLocalizationProvider.Tests.Refactoring.OldModelWithDisplayProperty.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.OldModelWithDisplayProperty.NewProperty-Description")]
    [InlineData(typeof(RenamedModelWithValidationClassAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationClassAndNamespace.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationClassAndNamespace.NewProperty-Required",
                "In.Galaxy.Far.Far.Away.OldModelWithValidationClassAndNamespace.NewProperty",
                "In.Galaxy.Far.Far.Away.OldModelWithValidationClassAndNamespace.NewProperty-Required")]
    [InlineData(typeof(RenamedModelWithValidationAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationAndNamespace.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationAndNamespace.NewProperty-Required",
                "In.Galaxy.Far.Far.Away.RenamedModelWithValidationAndNamespace.NewProperty",
                "In.Galaxy.Far.Far.Away.RenamedModelWithValidationAndNamespace.NewProperty-Required")]
    [InlineData(typeof(RenamedModelWithValidationProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationProperty.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationProperty.NewProperty-Required",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationProperty.OldProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationProperty.OldProperty-Required")]
    [InlineData(typeof(RenamedModelWithDescriptionProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDescriptionProperty.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDescriptionProperty.NewProperty-Description",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDescriptionProperty.OldProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithDescriptionProperty.OldProperty-Description")]
    [InlineData(typeof(RenamedModelClassWithValidationProperty),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassWithValidationProperty.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassWithValidationProperty.NewProperty-Required",
                "DbLocalizationProvider.Tests.Refactoring.OldModelClassWithValidationProperty.OldProperty",
                "DbLocalizationProvider.Tests.Refactoring.OldModelClassWithValidationProperty.OldProperty-Required")]
    [InlineData(typeof(RenamedModelClassWithValidationPropertyAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassWithValidationPropertyAndNamespace.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelClassWithValidationPropertyAndNamespace.NewProperty-Required",
                "In.Galaxy.Far.Far.Away.OldModelClassWithValidationPropertyAndNamespace.OldProperty",
                "In.Galaxy.Far.Far.Away.OldModelClassWithValidationPropertyAndNamespace.OldProperty-Required")]
    [InlineData(typeof(RenamedModelWithValidationPropertyAndNamespace),
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationPropertyAndNamespace.NewProperty",
                "DbLocalizationProvider.Tests.Refactoring.RenamedModelWithValidationPropertyAndNamespace.NewProperty-Required",
                "In.Galaxy.Far.Far.Away.RenamedModelWithValidationPropertyAndNamespace.OldProperty",
                "In.Galaxy.Far.Far.Away.RenamedModelWithValidationPropertyAndNamespace.OldProperty-Required")]
    public void ModelAdditionalResourceProperRenameDiscoveryTests(
        Type target,
        string resourceKey,
        string secondResourceKey,
        string oldResourceKey,
        string oldSecondResourceKey)
    {
        var result = _sut.ScanResources(target);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
        var discoveredResource = result.First(dr => dr.Key == resourceKey);
        var secondDiscoveredResource = result.First(dr => dr.Key == secondResourceKey);

        Assert.Equal(resourceKey, discoveredResource.Key);
        Assert.Equal(oldResourceKey, discoveredResource.OldResourceKey);
        Assert.Equal(secondResourceKey, secondDiscoveredResource.Key);
        Assert.Equal(oldSecondResourceKey, secondDiscoveredResource.OldResourceKey);
    }

    [Theory]
    [InlineData(typeof(RenamedResourceClassWithAdditionalAttribute),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassWithAdditionalAttribute.NewResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassWithAdditionalAttribute.NewResourceKey-AdditionalData",
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClassWithAdditionalAttribute.NewResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClassWithAdditionalAttribute.NewResourceKey-AdditionalData")]
    [InlineData(typeof(RenamedResourceKeyWithAdditionalAttribute),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKeyWithAdditionalAttribute.NewResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKeyWithAdditionalAttribute.NewResourceKey-AdditionalData",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKeyWithAdditionalAttribute.OldResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceKeyWithAdditionalAttribute.OldResourceKey-AdditionalData")]
    [InlineData(typeof(RenamedResourceClassAndKeyWithAdditionalAttribute),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKeyWithAdditionalAttribute.NewResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKeyWithAdditionalAttribute.NewResourceKey-AdditionalData",
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClassAndKeyWithAdditionalAttribute.OldResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.OldResourceClassAndKeyWithAdditionalAttribute.OldResourceKey-AdditionalData")]
    [InlineData(typeof(RenamedResourceClassAndKeyAndNamespaceWithAdditionalAttribute),
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKeyAndNamespaceWithAdditionalAttribute.NewResourceKey",
                "DbLocalizationProvider.Tests.Refactoring.RenamedResourceClassAndKeyAndNamespaceWithAdditionalAttribute.NewResourceKey-AdditionalData",
                "In.Galaxy.Far.Far.Away.OldResourceClassAndKeyAndNamespaceWithAdditionalAttribute.OldResourceKey",
                "In.Galaxy.Far.Far.Away.OldResourceClassAndKeyAndNamespaceWithAdditionalAttribute.OldResourceKey-AdditionalData")]
    public void ModelCustomAttributesResourceProperRenameDiscoveryTests(
        Type target,
        string resourceKey,
        string secondResourceKey,
        string oldResourceKey,
        string oldSecondResourceKey)
    {
        var result = _sut.ScanResources(target);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
        var discoveredResource = result.First(dr => dr.Key.EndsWith("NewResourceKey"));
        var secondDiscoveredResource = result.First(dr => dr.Key.EndsWith("AdditionalData"));

        Assert.Equal(resourceKey, discoveredResource.Key);
        Assert.Equal(oldResourceKey, discoveredResource.OldResourceKey);
        Assert.Equal(secondResourceKey, secondDiscoveredResource.Key);
        Assert.Equal(oldSecondResourceKey, secondDiscoveredResource.OldResourceKey);
    }
}
