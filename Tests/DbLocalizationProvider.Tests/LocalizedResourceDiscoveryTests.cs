using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedResourceDiscoveryTests
    {

        private readonly List<Type> _types;
        private readonly TypeDiscoveryHelper _sut;
        private readonly ExpressionHelper _expressionHelper;

        public LocalizedResourceDiscoveryTests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);

            _expressionHelper = new ExpressionHelper(keyBuilder);

            _types = _sut.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            Assert.NotEmpty(_types);
        }

        [Fact]
        public void NestedObject_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.ResourceKeys");
            var properties = _sut.ScanResources(type).ToList();

            var complexPropertySubProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.ResourceKeys.SubResource.SubResourceProperty");

            Assert.NotNull(complexPropertySubProperty);
            Assert.Equal("Sub Resource Property", complexPropertySubProperty.Translations.DefaultTranslation());

            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.AnotherResource", properties.Select(k => k.Key));
            Assert.Contains("DbLocalizationProvider.Tests.ResourceKeys.SubResource.EvenMoreComplexResource.Amount", properties.Select(k => k.Key));

            // need to check that there is no resource discovered for complex properties itself
            Assert.DoesNotContain("DbLocalizationProvider.Tests.ResourceKeys.SubResource", properties.Select(k => k.Key));
            Assert.DoesNotContain("DbLocalizationProvider.Tests.ResourceKeys.SubResource.EvenMoreComplexResource", properties.Select(k => k.Key));
        }

        [Fact]
        public void NestedType_ScalarProperties()
        {
            var type = _types.FirstOrDefault(t => t.FullName == "DbLocalizationProvider.Tests.ParentClassForResources+ChildResourceClass");

            Assert.NotNull(type);

            var property = _sut.ScanResources(type).First();
            var resourceKey = _expressionHelper.GetFullMemberName(() => ParentClassForResources.ChildResourceClass.HelloMessage);

            Assert.Equal(resourceKey, property.Key);
        }

        [Fact]
        public void NestedType_ThroughProperty_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.PageResources");

            Assert.NotNull(type);

            var property = _sut.ScanResources(type).FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.PageResources.Header.HelloMessage");

            Assert.NotNull(property);
        }

        [Fact]
        public void SingleLevel_ScalarProperties()
        {
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.ResourceKeys");
            var properties = _sut.ScanResources(type);

            var staticField = properties.First(p => p.Key == "DbLocalizationProvider.Tests.ResourceKeys.ThisIsConstant");

            Assert.True(LocalizedTypeScannerBase.IsStringProperty(staticField.ReturnType));
            Assert.Equal("Default value for constant", staticField.Translations.DefaultTranslation());
        }
    }
}
