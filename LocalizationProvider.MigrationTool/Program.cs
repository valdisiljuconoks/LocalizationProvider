using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using NDesk.Options;
using TechFellow.DbLocalizationProvider;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var showHelp = false;
            string sourceDirectory = null;
            string targetDirectory = null;

            var p = new OptionSet
            {
                {
                    "s|sourceDir=", "web application source directory",
                    v => sourceDirectory = v
                },
                {
                    "t|targetDir=", "target directory where to write import script",
                    v => targetDirectory = v
                },
                {
                    "h|help", "show this message and exit",
                    v => showHelp = v != null
                }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("LocalizationProvider.MigrationTool: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `LocalizationProvider.MigrationTool.exe --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }

            if (string.IsNullOrEmpty(sourceDirectory))
            {
                Console.WriteLine("ERROR: Source directory parameter is missing!");
                Console.WriteLine();
                ShowHelp(p);
                return;
            }

            if (string.IsNullOrEmpty(targetDirectory))
            {
                targetDirectory = sourceDirectory;
            }

            if (!Directory.Exists(sourceDirectory))
            {
                throw new IOException($"Source directory {sourceDirectory} does not exist!");
            }

            Console.WriteLine("Improt started!");
            Directory.SetCurrentDirectory(sourceDirectory);

            var resourceFilesSourceDir = Path.Combine(sourceDirectory, "Resources\\LanguageFiles");

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
            var resources = fileProcessor.ParseFiles(resourceFiles);

            var vdm = new VirtualDirectoryMapping(targetDirectory, true);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            var config = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");

            var connectionString = config.ConnectionStrings.ConnectionStrings["EPiServerDB"].ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException("Cannot find EPiServer database connection");
            }

            // in case application is running with LocalDb
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(sourceDirectory, "App_Data"));

            // initialize DB - to generate data structures
            using (var db = new LanguageEntities(connectionString))
            {
                var resource = db.LocalizationResources.Where(r => r.Id == 0);
            }

            var sb = new StringBuilder();

            sb.AppendLine("declare @id int;");

            foreach (var resourceEntry in resources)
            {
                sb.AppendLine($"insert dbo.LocalizationResources values (N'{resourceEntry.Key.Replace("'", "''")}', getdate(), 'migration');");
                sb.AppendLine("set @id=IDENT_CURRENT('dbo.LocalizationResources');");

                foreach (var resourceTranslation in resourceEntry.Translations)
                {
                    sb.AppendLine($"insert dbo.LocalizationResourceTranslations (resourceid, language, value) values (@id, '{resourceTranslation.CultureId}', N'{resourceTranslation.Translation.Replace("'", "''")}');");
                }
            }

            // clear previous state (if any)

            if (File.Exists(Path.Combine(targetDirectory, "localization-resource-translations.sql")))
            {
                File.Delete(Path.Combine(targetDirectory, "localization-resource-translations.sql"));
            }

            using (var outputFile = File.Open(Path.Combine(targetDirectory, "localization-resource-translations.sql"), FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(outputFile))
                {
                    writer.Write(sb.ToString());
                }
            }

            /*
            
            
            declare @id int;

            insert dbo.LocalizationResources values ('testkey', getdate(), 'migration');
            set @id=IDENT_CURRENT('dbo.LocalizationResources');
            insert dbo.LocalizationResourceTranslations values (@id, 'en', 'English');

            */

            Console.WriteLine("Export completed!");
            Console.ReadLine();
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: LocalizationProvider.MigrationTool.exe [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");

            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
