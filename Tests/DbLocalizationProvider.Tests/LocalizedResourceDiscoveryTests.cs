using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedResourceDiscoveryTests
    {
        public LocalizedResourceDiscoveryTests()
        {
            _sut = new TypeDiscoveryHelper();
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            _types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            Assert.NotEmpty(_types);
        }

        private readonly List<Type> _types;
        private readonly TypeDiscoveryHelper _sut;

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
            var resourceKey = ExpressionHelper.GetFullMemberName(() => ParentClassForResources.ChildResourceClass.HelloMessage);

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
            var sut = new TypeDiscoveryHelper();
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.ResourceKeys");
            var properties = sut.ScanResources(type);

            var staticField = properties.First(p => p.Key == "DbLocalizationProvider.Tests.ResourceKeys.ThisIsConstant");

            Assert.True(LocalizedTypeScannerBase.IsStringProperty(staticField.ReturnType));
            Assert.Equal("Default value for constant", staticField.Translations.DefaultTranslation());
        }
    }
}
