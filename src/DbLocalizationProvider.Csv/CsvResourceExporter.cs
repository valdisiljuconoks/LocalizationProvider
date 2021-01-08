using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DbLocalizationProvider.Export;

namespace DbLocalizationProvider.Csv
{
    public class CsvResourceExporter : IResourceExporter
    {
        private readonly Func<ICollection<CultureInfo>> _languagesFactory;

        public CsvResourceExporter() : this(null)
        {
        }

        public CsvResourceExporter(Func<ICollection<CultureInfo>> languagesFactory)
        {
            _languagesFactory = languagesFactory;
        }
        
        /// <summary>
        /// Gets the name of the export format (this will be visible on menu).
        /// </summary>
        public string FormatName => "CSV";
        
        /// <summary>
        /// Gets the export provider identifier.
        /// </summary>
        public string ProviderId => "csv";

        /// <summary>
        /// Exports the specified resources.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// Result of the export
        /// </returns>
        public ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters)
        {
            var records = new List<object>();
            var languages = GetLanguages(resources);

            foreach (var resource in resources.OrderBy(x => x.ResourceKey))
            {
                dynamic record = new ExpandoObject();

                record.ResourceKey = resource.ResourceKey;

                foreach (var language in languages)
                {
                    var translation = resource.Translations.ByLanguage(language.Name, false);
                    AddProperty(record, language.Name, translation);
                }

                records.Add(record);
            }

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
            
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, csvConfig))
            {
                csv.WriteRecords(records);
                streamWriter.Flush();
                var bytes = stream.ToArray();
                var csvContent = Encoding.UTF8.GetString(bytes);
                var fileName = $"localization-resources-{DateTime.UtcNow:yyyyMMdd}.csv";
                return new ExportResult(csvContent, "text/csv", fileName);
            }
        }

        private ICollection<CultureInfo> GetLanguages(ICollection<LocalizationResource> resources)
        {
            if (_languagesFactory != null)
            {
                return _languagesFactory();
            }

            return resources
                .SelectMany(x => x.Translations)
                .Select(x => x.Language)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .Select(x => TryGetCulture(x))
                .Where(x => x != null)
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

        private void AddProperty(ExpandoObject record, string languageName, string translation)
        {
            var recordDict = record as IDictionary<string, object>;

            if (recordDict.ContainsKey(languageName))
            {
                recordDict[languageName] = translation;
            }
            else
            {
                recordDict.Add(languageName, translation);
            }
        }
    }
}
