using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class DiscoveryTests
    {
        [Fact]
        public void SingleLevel_ScalarProperties_NoAttributes()
        {
            var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>().ToList();

            Assert.NotEmpty(types);

            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type);

            Assert.Contains("DbLocalizationProvider.Tests.KeyModel.SampleProperty", properties);

            //foreach (var property in properties)
            //{
            //    var z1 = property.Name;

            //    string resourceKey = $"{type.FullName}.{z1}";

            //    resourceKeys.Add(resourceKey);

            //var attributes = property.GetCustomAttributes(false);

            //foreach (var attribute in attributes)
            //{
            //    var attributeType = attribute.GetType();
            //    var resourceKeyFragment = string.Empty;

            //    if (attributeType == typeof(DisplayNameAttribute)
            //        || attributeType.IsSubclassOf(typeof(DisplayNameAttribute))
            //        || attributeType == typeof(ValidationAttribute)
            //        || attributeType.IsSubclassOf(typeof(ValidationAttribute)))
            //    {
            //        resourceKeyFragment = attributeType.Name;
            //    }

            //    if (!string.IsNullOrEmpty(resourceKeyFragment))
            //    {
            //        resourceKeyFragment = attributeType.Name.Replace("Attribute", string.Empty).ToLower();
            //        resourceKeys.Add($"{resourceKey}-{resourceKeyFragment}");
            //    }
            //}
            //}

            //Assert.NotEmpty(resourceKeys);
        }
    }
}
