using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.EnumTests
{
    public class LocalizedEnumTests
    {
        public LocalizedEnumTests()
        {
            var types = new[] { typeof(DocumentEntity) };
            var sut = new TypeDiscoveryHelper();

            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => sut.ScanResources(t));
        }

        private readonly IEnumerable<DiscoveredResource> _properties;

        [Fact]
        public void DiscoverEnumValue_NameAsTranslation()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleStatus));

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open");

            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public void DiscoverEnumWithPrefixKey()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleStatusWithPrefix));

            var openStatus = properties.First(p => p.Key == "ThisIsPrefix.Open");

            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public void EnumType_CheckDiscovered_Found()
        {
            var enumProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status");
            Assert.NotNull(enumProperty);
        }

        [Fact]
        public void EnumWithDisplayAttribute_TranslationEqualToSpecifiedInAttribute()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleEnumWithDisplayAttribute));

            var newStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithDisplayAttribute.New");

            Assert.Equal("This is new", newStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public void EnumWithAdditionalTranslation_DiscoversAllTranslations()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleEnumWithAdditionalTranslations));

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithAdditionalTranslations.Open");

            Assert.Equal(openStatus.Translations.Count, 3);
            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
            Assert.Equal("Åpen", openStatus.Translations.First(t => t.Culture == "no").Translation);
        }

        [Fact]
        public void EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(SampleEnumWithKeys));

            Assert.NotEmpty(discoveredResources);

            Assert.True(discoveredResources.Any(r => r.Key == "/this/is/key"));
        }

        [Fact]
        public void EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames_WithClassPrefix()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(SampleEnumWithKeysWithClassPrefix));

            Assert.NotEmpty(discoveredResources);

            Assert.True(discoveredResources.Any(r => r.Key == "/this/is/prefix/and/this/is/key"));
        }

        [Fact]
        public void Test_EnumExpression_AsProperty()
        {
            var doc = new DocumentEntity
            {
                Status = SampleStatus.New
            };

            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status", ExpressionHelper.GetFullMemberName(() => doc.Status));
        }

        [Fact]
        public void Test_EnumExpression_Directly()
        {
            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open", ExpressionHelper.GetFullMemberName(() => SampleStatus.Open));
        }

        [Fact]
        public void Test_MemberAccessExpression()
        {
            var doc = new DocumentEntity();
            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity", ExpressionHelper.GetFullMemberName(() => doc));
        }
    }
}
