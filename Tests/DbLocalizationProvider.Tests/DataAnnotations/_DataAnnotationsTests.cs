using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class DataAnnotationsTests
    {
        public DataAnnotationsTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>()
                                .SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void AdditionalCustomAttributesTest()
        {
            ConfigurationContext.Current.TypeFactory
                                .ForQuery<GetTranslation.Query>()
                                .SetHandler<GetTranslationReturnResourceKeyHandler>();

            var sut = new LocalizationProvider();

            var result = sut.GetString(() => ResourceClassWithCustomAttributes.Resource1, typeof(CustomRegexAttribute));

            Assert.Equal("DbLocalizationProvider.Tests.DataAnnotations.ResourceClassWithCustomAttributes.Resource1-CustomRegex", result);
        }

        [Fact]
        public void ChildClassTypeAttributeUsage_ShouldRegisterResource()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ViewModelWithInheritedDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
        }

        [Fact]
        public void DirectDataTypeAttributeUsage_ShouldNotRegisterResource()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ViewModelWithSomeDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(1, properties.Count());
        }
    }

    public class GetTranslationReturnResourceKeyHandler : IQueryHandler<GetTranslation.Query, string>
    {
        public string Execute(GetTranslation.Query query)
        {
            return query.Key;
        }
    }

    public class CustomRegexAttribute : RegularExpressionAttribute
    {
        public CustomRegexAttribute(string pattern) : base(pattern)
        {
        }
    }

    [LocalizedResource]
    public class ResourceClassWithCustomAttributes
    {
        [CustomRegex(".")]
        public static string Resource1 { get; set; } = "Resource 1";
    }
}
