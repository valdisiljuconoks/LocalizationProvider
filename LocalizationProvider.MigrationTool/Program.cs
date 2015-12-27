using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
            var sourceDirectory = string.Empty;
            var targetDirectory = string.Empty;
            var scriptUpdate = false;

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
                    "u|updateResources=", "Generate update script statements for existing resources",
                    v => bool.TryParse(v, out scriptUpdate)
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
                Console.WriteLine("Try 'LocalizationProvider.MigrationTool.exe --help' for more information.");
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

            Console.WriteLine("Import started!");
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

            // generate migration script
            var scriptGenerator = new ScriptGenerator();
            var generatedScript = scriptGenerator.Generate(resources, scriptUpdate);

            // write migration script to file
            var scriptFileWriter = new ScriptFileWriter();
            scriptFileWriter.Write(generatedScript, targetDirectory);

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
