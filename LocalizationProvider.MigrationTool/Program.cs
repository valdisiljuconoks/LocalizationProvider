using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

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
