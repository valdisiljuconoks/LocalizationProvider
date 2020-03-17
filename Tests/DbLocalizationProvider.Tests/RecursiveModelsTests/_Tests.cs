using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.RecursiveModelsTests
{
    public class _Tests
    {
        [Fact]
        public void Model_WithTheSameModelAsProperty_ShouldThrow()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<RecursiveResourceReferenceException>(() =>
            {
                var resources = sut.ScanResources(typeof(Person));
            });
        }

        [Fact]
        public void Model_WithObjectProperty_ShouldNotThrow()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            var sut = new TypeDiscoveryHelper();

            var resources = sut.ScanResources(typeof(ResourceClassWithObjectTypeProperty));
        }
    }

    [LocalizedModel]
    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Person Mother { get; set; }
        public Person Father { get; set; }
    }

    [LocalizedModel]
    public class ResourceClassWithObjectTypeProperty
    {
        public object SomeObject { get; set; } = "test";
    }
}
