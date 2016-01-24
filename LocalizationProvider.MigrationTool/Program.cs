using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using NDesk.Options;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class Program
    {
        private static MigrationToolSettings _settings;

        public static void Main(string[] args)
        {
            _settings = ParseArguments(args);

            if (_settings.ShowHelp)
            {
                ShowHelp(_settings.OptionSet);
                return;
            }

            if (string.IsNullOrEmpty(_settings.SourceDirectory))
            {
                Console.WriteLine("ERROR: Source directory parameter is missing!");
                Console.WriteLine();
                ShowHelp(_settings.OptionSet);
                return;
            }

            if (!Directory.Exists(_settings.SourceDirectory))
            {
                throw new IOException($"Source directory {_settings.SourceDirectory} does not exist!");
            }

            Directory.SetCurrentDirectory(_settings.SourceDirectory);
            ReadConnectionString(_settings);
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_settings.SourceDirectory, "App_Data"));

            if (_settings.ExportResources)
            {
                Console.WriteLine("Export started.");
                var extractor = new ResourceExtractor();
                var resources = extractor.Extract(_settings);

                var scriptGenerator = new ScriptGenerator();
                var generatedScript = scriptGenerator.Generate(resources, _settings.ScriptUpdate);

                var scriptFileWriter = new ScriptFileWriter();
                var outputFile = scriptFileWriter.Write(generatedScript, _settings.TargetDirectory);

                Console.WriteLine($"Output file: {outputFile}");
                Console.WriteLine("Export completed!");
            }

            if (_settings.ImportResources)
            {
                Console.WriteLine("Import started!");

                var importer = new ResourceImporter();
                importer.Import(_settings);

                Console.WriteLine("Import completed!");
            }

            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }
        }

        private static void ReadConnectionString(MigrationToolSettings settings)
        {
            var vdm = new VirtualDirectoryMapping(_settings.SourceDirectory, true);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            var config = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");

            var connectionString = config.ConnectionStrings.ConnectionStrings["EPiServerDB"].ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException("Cannot find EPiServer database connection");
            }

            settings.ConnectionString = connectionString;
        }

        private static MigrationToolSettings ParseArguments(string[] args)
        {
            var showHelp = false;
            var sourceDirectory = string.Empty;
            var targetDirectory = string.Empty;
            var scriptUpdate = false;
            var exportResources = false;
            var importResources = false;
            var exportFromDatabase = false;

            var p = new OptionSet
            {
                {
                    "s|sourceDir=", "web application source directory",
                    v => sourceDirectory = v
                },
                {
                    "t|targetDir=", "Target directory where to write import script (by default 'sourceDir')",
                    v => targetDirectory = v
                },
                {
                    "o|overwriteResources", "Generate update script statements for existing resources",
                    k => scriptUpdate = true
                },
                {
                    "e|exportResources", "Export localization resources",
                    k => exportResources = true
                },
                {
                    "from-db|fromDatabase", "Export localization resources from SQL database",
                    k => exportFromDatabase = true
                },
                {
                    "i|importResources", "Import localization resources from SQL file into database",
                    k => importResources = true
                },
                {
                    "h|help", "show this message and exit",
                    v => showHelp = v != null
                }
            };

            var result = new MigrationToolSettings(p);

            try
            {
                var extra = p.Parse(args);
                result.SourceDirectory = sourceDirectory;
                result.TargetDirectory = targetDirectory;
                result.ScriptUpdate = scriptUpdate;
                result.ExportResources = exportResources;
                result.ExportFromDatabase = exportFromDatabase;
                result.ImportResources = importResources;
                result.ShowHelp = showHelp;
            }
            catch (OptionException e)
            {
                Console.Write("LocalizationProvider.MigrationTool: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'LocalizationProvider.MigrationTool.exe --help' for more information.");
            }

            if (string.IsNullOrEmpty(result.TargetDirectory))
            {
                result.TargetDirectory = result.SourceDirectory;
            }

            return result;
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
