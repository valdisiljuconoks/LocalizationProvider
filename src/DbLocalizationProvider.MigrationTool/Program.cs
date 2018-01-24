using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using DbLocalizationProvider.Export;
using NDesk.Options;

namespace DbLocalizationProvider.MigrationTool
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
                throw new IOException($"Source directory `{_settings.SourceDirectory}` does not exist!");

            Configuration config;

            if(File.Exists(Path.Combine(_settings.SourceDirectory, "web.config")))
            {
                Directory.SetCurrentDirectory(_settings.SourceDirectory);
                config = GetWebConfig();
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_settings.SourceDirectory, "App_Data"));
            }
            else if(File.Exists(Path.Combine(_settings.SourceDirectory, "app.config")))
            {
                Directory.SetCurrentDirectory(_settings.SourceDirectory);
                config = ReadAppConnectionString();
            }
            else
                throw new IOException($"Neither `web.config` nor `app.config` file not found in `{_settings.SourceDirectory}`!");

            var connectionString = config.ConnectionStrings?.ConnectionStrings["EPiServerDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ConfigurationErrorsException("Could not find EPiServer database connection.");

            ConfigurationContext.Current.DbContextConnectionString = _settings.ConnectionString = connectionString;

            if (_settings.ExportResources)
            {
                try
                {
                    Console.WriteLine("Export started.");
                    var extractor = new ResourceExtractor();
                    var resources = extractor.Extract(_settings);
                    string generatedScript;

                    if (_settings.Json)
                    {
                        var serializer = new JsonResourceExporter();
                        generatedScript = serializer.Export(resources).SerializedData;
                    }
                    else
                    {
                        var scriptGenerator = new SqlScriptGenerator();
                        generatedScript = scriptGenerator.Generate(resources, _settings.ScriptUpdate);
                    }

                    var scriptFileWriter = new ResultFileWriter();
                    var outputFile = scriptFileWriter.Write(generatedScript, _settings.TargetDirectory, _settings.Json);

                    Console.WriteLine($"Output file: {outputFile}");
                    Console.WriteLine("Export completed!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error running tool: {e.Message}");
                    return;
                }
            }

            if (_settings.ImportResources)
            {
                Console.WriteLine("Import started!");

                var importer = new ResourceImporter();
                importer.Import(_settings);

                Console.WriteLine("Import completed!");
            }

            if (!_settings.ExportResources && !_settings.ImportResources)
            {
                Console.WriteLine("No command specified.");
                Console.WriteLine("Try 'DbLocalizationProvider.MigrationTool.exe --help' for more information.");
            }

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        private static Configuration ReadAppConnectionString()
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = "app.config" }, ConfigurationUserLevel.None);

            return config;
        }

        private static Configuration GetWebConfig()
        {
            var vdm = new VirtualDirectoryMapping(_settings.SourceDirectory, true);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);

            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }

        private static MigrationToolSettings ParseArguments(string[] args)
        {
            var showHelp = false;
            var sourceDirectory = string.Empty;
            var resourceDirectory = string.Empty;
            var targetDirectory = string.Empty;
            var scriptUpdate = false;
            var exportResources = false;
            var importResources = false;
            var exportFromDatabase = false;
            var jsonFormat = false;

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
                    "resourceDir=", "Xml language resource directory (relative to `sourceDir`, by default `Resources\\LanguageFiles`)",
                    v => resourceDirectory = v
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
                    "json|jsonFormat", "Use JSON format",
                    k => jsonFormat = true
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
                result.ResourceDirectory = resourceDirectory;
                result.TargetDirectory = targetDirectory;
                result.ScriptUpdate = scriptUpdate;
                result.ExportResources = exportResources;
                result.ExportFromDatabase = exportFromDatabase;
                result.ImportResources = importResources;
                result.ShowHelp = showHelp;
                result.Json = jsonFormat;
            }
            catch (OptionException e)
            {
                Console.Write("DbLocalizationProvider.MigrationTool: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'DbLocalizationProvider.MigrationTool.exe --help' for more information.");
            }

            if (string.IsNullOrEmpty(result.TargetDirectory))
                result.TargetDirectory = result.SourceDirectory;

            return result;
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: DbLocalizationProvider.MigrationTool.exe [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");

            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
