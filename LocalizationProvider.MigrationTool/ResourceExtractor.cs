using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using DbLocalizationProvider;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    internal class ResourceExtractor
    {
        internal ICollection<LocalizationResource> Extract(MigrationToolSettings settings)
        {
            
            if (settings.ExportFromDatabase)
            {
                using (var db = new LanguageEntities(settings.ConnectionString))
                {
                   return db.LocalizationResources.Include(r => r.Translations).ToList();
                }
            }
            else
            {
                ICollection<LocalizationResource> resources;

                // TODO: read this from the config
                var resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, "Resources\\LanguageFiles");
                if (!Directory.Exists(resourceFilesSourceDir))
                {
                    throw new IOException($"Resource directory '{resourceFilesSourceDir}' does not exist!");
                }

                var resourceFiles = Directory.GetFiles(resourceFilesSourceDir, "*.xml");
                if (!resourceFiles.Any())
                {
                    Console.WriteLine($"No resource files found in '{resourceFilesSourceDir}'");
                }

                var fileProcessor = new ResourceFileProcessor();
                resources = fileProcessor.ParseFiles(resourceFiles);

                // initialize DB - to generate data structures
                try
                {
                    using (var db = new LanguageEntities(settings.ConnectionString))
                    {
                        var resource = db.LocalizationResources.Where(r => r.Id == 0);
                    }
                }
                catch
                {
                    // it's OK to have exception here
                }

                return resources;
            }
        }
    }
}
