using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider.Csv
{
    public class CsvResourceFormatParser : IResourceFormatParser
    {
        private readonly Func<ICollection<CultureInfo>> _languagesFactory;

        public CsvResourceFormatParser() : this(null)
        {
        }

        public CsvResourceFormatParser(Func<ICollection<CultureInfo>> languagesFactory)
        {
            _languagesFactory = languagesFactory;
        }

        /// <summary>
        ///     Gets the name of the format.
        /// </summary>
        public string FormatName => "CSV";
        
        /// <summary>
        ///     Gets the supported file extensions.
        /// </summary>
        public string[] SupportedFileExtensions => new[] {".csv"};
        
        /// <summary>
        ///     Gets the provider identifier.
        /// </summary>
        public string ProviderId => "csv";

        /// <summary>
        ///     Parses the specified file content.
        /// </summary>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns>
        ///     Returns list of resources from the file
        /// </returns>
        public ParseResult Parse(string fileContent)
        {
            var resources = new List<LocalizationResource>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

            using (var stream = AsStream(fileContent))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                var languages = GetLanguages(records);

                foreach (var record in records)
                {
                    var dict = (IDictionary<string, object>)record;
                    var resourceKey = dict["ResourceKey"] as string;
                    var resource = new LocalizationResource(resourceKey)
                    {
                        Translations = CreateTranslations(record, languages)
                    };
                    resources.Add(resource);
                }

                return new ParseResult(resources, languages);
            }

        }
        
        private ICollection<CultureInfo> GetLanguages(ICollection<dynamic> records)
        {
            if (_languagesFactory != null)
            {
                return _languagesFactory();
            }

            if (records.Count == 0)
            {
                return new List<CultureInfo>();
            }

            var firstResource = (IDictionary<string, object>)records.First();

            return firstResource
                   .Keys
                   .Where(x => !x.Equals("ResourceKey"))
                   .Select(x => TryGetCulture(x))
                   .ToList();
        }
        
        private CultureInfo TryGetCulture(string cultureName)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                return culture;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private ICollection<LocalizationResourceTranslation> CreateTranslations(IDictionary<string, object> record, IEnumerable<CultureInfo> languages)
        {
            return languages.Select(x => new LocalizationResourceTranslation
            {
                Language = x.Name,
                Value = record.ContainsKey(x.Name)
                    ? record[x.Name] as string
                    : null
            }).ToList();
        }

        private Stream AsStream(string fileContent)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContent);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
