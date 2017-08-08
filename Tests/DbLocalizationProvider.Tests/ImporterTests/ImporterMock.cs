using System.Collections.Generic;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class ImporterMock : IResourceImporter
    {
        private static readonly string[] _extensions = { ".ext1", ".ext2", ".ext3" };

        public string FormatName => "EXT";

        public string[] SupportedFileExtensions => _extensions;

        public string ProviderId => "ext";

        public ICollection<LocalizationResource> Parse(string fileContent)
        {
            return new List<LocalizationResource>();
        }
    }
}
