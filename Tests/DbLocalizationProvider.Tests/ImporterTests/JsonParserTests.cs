using DbLocalizationProvider.Import;
using Xunit;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class JsonParserTests
    {
        [Fact]
        public void TestDetectedLanguages()
        {
            var sut = new JsonResourceFormatParser();

            var result = sut.Parse(_sampleJson);

            // should detect `en` and `fi-FI` and not invariant one
            Assert.Equal(2, result.DetectedLanguages.Count);
        }


        private static string _sampleJson = @"
[
  {
    ""id"": 1303,
    ""resourceKey"": ""DbLocalizationProvider.EPiServer.Sample.ClientSideResources.Class2.SomeValue"",
    ""modificationDate"": ""2017-06-15T22:36:24.687Z"",
    ""author"": ""type-scanner"",
    ""fromCode"": true,
    ""isModified"": true,
    ""isHidden"": false,
    ""translations"": [
      {
        ""id"": 1480,
        ""resourceId"": 1303,
        ""language"": ""en"",
        ""value"": ""Some value 2""
      },
      {
        ""resourceId"": 1303,
        ""language"": ""fi-FI"",
        ""value"": ""SOME VALUE (fi-FI)""
      },
      {
        ""id"": 1481,
        ""resourceId"": 1303,
        ""language"": """",
        ""value"": ""Some value""
      }
    ]
  },
]
";
    }
}
