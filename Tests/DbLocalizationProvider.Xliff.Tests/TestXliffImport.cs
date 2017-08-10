using System.Linq;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests
{
    public class TestXliffImport
    {
        private static string _sampleFile = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en"" trgLang=""no"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
	<file id=""f1"">
		<unit id=""u1"">
			<segment id=""My.Resource.Key"">
				<source>
					<![CDATA[this is english text]]>
				</source>
				<target>
					<![CDATA[det er tekst i norsk]]>
				</target>
			</segment>
			<segment id=""My.Resource.AnotherKey"">
				<source>
					<![CDATA[this is another english text]]>
				</source>
				<target>
					<![CDATA[det er andre tekst i norsk]]>
				</target>
			</segment>
		</unit>
	</file>
</xliff>
";
        [Fact]
        public void TestParse_ValidFile()
        {
            var sut = new FormatParser();

            var result = sut.Parse(_sampleFile);
            
            Assert.NotEmpty(result.Resources);
            Assert.Equal(2, result.Resources.Count);
            Assert.Equal("no", result.Resources.First().Translations.Single().Language);
        }
    }
}
