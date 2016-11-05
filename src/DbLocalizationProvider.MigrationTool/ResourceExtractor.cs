using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace DbLocalizationProvider.MigrationTool
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

            // test few default conventions (lazy enough to read from EPiServer Framework configuration file)
            string resourceFilesSourceDir;
            if (!string.IsNullOrEmpty(settings.ResourceDirectory))
            {
                resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, settings.ResourceDirectory);
            }
            else
            {
                resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, "Resources\\LanguageFiles");
                if (!Directory.Exists(resourceFilesSourceDir))
                {
                    resourceFilesSourceDir = Path.Combine(settings.SourceDirectory, "lang");
                }
            }

            if (!Directory.Exists(resourceFilesSourceDir))
            {
                throw new IOException($"Default resource directory '{resourceFilesSourceDir}' does not exist or can't be found. Use `-resourceDir` argument");
            }

            var resourceFiles = Directory.GetFiles(resourceFilesSourceDir, "*.xml");
            if (!resourceFiles.Any())
            {
                Console.WriteLine($"No resource files found in '{resourceFilesSourceDir}'");
            }

            var fileProcessor = new ResourceFileProcessor();
            var resources = fileProcessor.ParseFiles(resourceFiles);

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
