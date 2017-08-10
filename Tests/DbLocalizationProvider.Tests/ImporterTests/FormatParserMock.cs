using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class FormatParserMock : IResourceFormatParser
    {
        private static readonly string[] _extensions = { ".ext1", ".ext2", ".ext3" };

        public string FormatName => "EXT";

        public string[] SupportedFileExtensions => _extensions;

        public string ProviderId => "ext";

        public ParseResult Parse(string fileContent)
        {
            return new ParseResult(new List<LocalizationResource>(), new List<CultureInfo>());
        }
    }
}
