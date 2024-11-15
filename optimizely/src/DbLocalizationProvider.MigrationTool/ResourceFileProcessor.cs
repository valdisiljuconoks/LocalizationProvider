// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DbLocalizationProvider.MigrationTool
{
    internal class ResourceFileProcessor
    {
        private readonly XmlDocumentParser _parser;
        private readonly ResourceListMerger _mergeTool;
        private readonly bool _ignoreDuplicateKeys;

        public ResourceFileProcessor(bool ignoreDuplicateKeys)
        {
            _parser = new XmlDocumentParser();
            _mergeTool = new ResourceListMerger();
            _ignoreDuplicateKeys = ignoreDuplicateKeys;
        }

        public ICollection<LocalizationResource> ParseFiles(string[] resourceFiles)
        {
            if (!resourceFiles.Any())
            {
                throw new ArgumentException("Resource file list is empty", nameof(resourceFiles));
            }

            var result = new List<LocalizationResource>();

            foreach (var resourceFile in resourceFiles)
            {
                var stream = File.OpenText(resourceFile);
                var contentXml = XDocument.Load(stream);
                var resources = _parser.ReadXml(contentXml, _ignoreDuplicateKeys, resourceFile);

                result = _mergeTool.Merge(result, resources, _ignoreDuplicateKeys).ToList();
            }

            return result;
        }
    }
}
