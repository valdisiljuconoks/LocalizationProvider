using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    internal class ResourceFileProcessor
    {
        private readonly XmlDocumentParser _parser;
        private ResourceListMerger _mergeTool;

        public ResourceFileProcessor()
        {
            _parser = new XmlDocumentParser();
            _mergeTool = new ResourceListMerger();
        }

        public ICollection<ResourceEntry> ParseFiles(string[] resourceFiles)
        {
            if (!resourceFiles.Any())
            {
                throw new ArgumentException("Resource file list is empty", nameof(resourceFiles));
            }

            var result = new List<ResourceEntry>();

            foreach (var resourceFile in resourceFiles)
            {
                var stream = File.OpenText(resourceFile);
                var contentXml = XDocument.Load(stream);
                var resources = _parser.ReadXml(contentXml);

                result = _mergeTool.Merge(result, resources).ToList();
            }

            return result;
        }
    }
}
