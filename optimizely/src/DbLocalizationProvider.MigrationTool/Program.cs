// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using CommandLine;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Storage.SqlServer;

namespace DbLocalizationProvider.MigrationTool
{
    public class Program
    {
        private static MigrationToolOptions _settings;

        public static int Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.EnableDashDash = true;
                with.HelpWriter = Console.Error;
            });

            var result = parser.ParseArguments<MigrationToolOptions>(args).WithParsed(parsed =>
            {
                _settings = parsed;

                if (string.IsNullOrEmpty(parsed.TargetDirectory))
                {
                    _settings.TargetDirectory = parsed.SourceDirectory;
                }
            });

            if (result.Tag == ParserResultType.NotParsed) return -1;
            if (!Directory.Exists(_settings.SourceDirectory)) throw new IOException($"Source directory `{_settings.SourceDirectory}` does not exist!");

            try
            {
                if (_settings.ExportResources) ExportResources();
                if (_settings.ImportResources) ImportResources();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error running tool: {e.Message}");
                return -1;
            }

            if (!_settings.ExportResources && !_settings.ImportResources)
            {
                Console.WriteLine("No command specified. Please make up your mind, either you want to export or import resources.");
                Console.WriteLine("Try 'DbLocalizationProvider.MigrationTool.exe --help' for more information.");
            }

            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }

            return 0;
        }

        private static void ExportResources()
        {
            if (_settings.ExportFromDatabase) { SetConnectionString(); }

            Console.WriteLine("Starting export...");
            var extractor = new ResourceExporter();
            var resources = extractor.Export(_settings);
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

        private static void ImportResources()
        {
            Console.WriteLine("Starting import...");

            SetConnectionString();
            var importer = new ResourceImporter();
            importer.Import(_settings);

            Console.WriteLine("Import completed!");
        }

        private static void SetConnectionString()
        {
            if (!string.IsNullOrEmpty(_settings.ConnectionString))
            {
                Settings.DbContextConnectionString = _settings.ConnectionString;
                return;
            }

            Configuration config;

            if (File.Exists(Path.Combine(_settings.SourceDirectory, "web.config")))
            {
                Directory.SetCurrentDirectory(_settings.SourceDirectory);
                config = GetWebConfig();
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_settings.SourceDirectory, "App_Data"));
            }
            else if (File.Exists(Path.Combine(_settings.SourceDirectory, "app.config")))
            {
                Directory.SetCurrentDirectory(_settings.SourceDirectory);
                config = ReadAppConnectionString();
            }
            else
            {
                throw new IOException($"Neither `web.config` nor `app.config` file not found in `{_settings.SourceDirectory}`!");
            }

            var connectionString = config?.ConnectionStrings?.ConnectionStrings["EPiServerDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException("Could not find database connection  by name `EPiServerDB`");
            }

            Settings.DbContextConnectionString = _settings.ConnectionString = connectionString;
        }

        private static Configuration ReadAppConnectionString()
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap { ExeConfigFilename = "app.config" },
                ConfigurationUserLevel.None);

            return config;
        }

        private static Configuration GetWebConfig()
        {
            var vdm = new VirtualDirectoryMapping(_settings.SourceDirectory, true);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);

            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }
    }
}
