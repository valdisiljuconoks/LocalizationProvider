// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbLocalizationProvider.Storage.SqlServer;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.MigrationTool
{
    internal class ResourceExporter
    {
        internal ICollection<LocalizationResource> Export(MigrationToolOptions settings)
        {
            ICollection<LocalizationResource> resources = new List<LocalizationResource>();

            if(settings.ExportFromXmlOnly) resources = GetXmlResources(settings);
            if(settings.ExportFromDatabase)
            {
                var repo = new ResourceRepository();
                resources = repo.GetAll().ToList();

                InitializeDb(settings);
            }

            return resources;
        }

        private void InitializeDb(MigrationToolOptions settings)
        {
            try
            {
                var updater = new SchemaUpdater();
                updater.Execute(new UpdateSchema.Command());
            }
            catch
            {
                // it's OK to have exception here
            }
        }

        private ICollection<LocalizationResource> GetXmlResources(MigrationToolOptions settings)
        {
            // test few default conventions (lazy enough to read from EPiServer Framework configuration file)
            string resourceFilesSourceDir;
            if(!string.IsNullOrEmpty(settings.ResourceDirectory))
            {
                resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, settings.ResourceDirectory);
            }
            else
            {
                resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, "Resources\\LanguageFiles");
                if(!Directory.Exists(resourceFilesSourceDir))
                {
                    resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, "lang");
                }
            }

            if(!Directory.Exists(resourceFilesSourceDir))
            {
                throw new IOException($"Default resource directory '{resourceFilesSourceDir}' does not exist or can't be found. Use `-resourceDirectory` argument");
            }

            var resourceFiles = Directory.GetFiles(resourceFilesSourceDir, "*.xml");
            if(!resourceFiles.Any())
            {
                Console.WriteLine($"No resource files found in '{resourceFilesSourceDir}'");
            }

            var fileProcessor = new ResourceFileProcessor(settings.IgnoreDuplicateKeys);

            return fileProcessor.ParseFiles(resourceFiles);
        }
    }
}
