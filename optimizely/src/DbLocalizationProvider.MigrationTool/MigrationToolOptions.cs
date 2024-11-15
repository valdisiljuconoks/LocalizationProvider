// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DbLocalizationProvider.MigrationTool
{
    public class MigrationToolOptions
    {
        [Option('s', "sourceDir", HelpText = "EPiServer project source directory", Required = true)]
        public string SourceDirectory { get; set; }

        [Option('t', "targetDir", HelpText = "Target directory where to write the output file (by default 'sourceDir')")]
        public string TargetDirectory { get; set; }

        [Option("resourceDirectory", HelpText = "Xml language resource directory (relative to `sourceDir`)", Default = "Resources\\LanguageFiles")]
        public string ResourceDirectory { get; set; }

        [Option('o', "overwriteResources", HelpText = "Generate update statements for existing resources. Effective if not using JSON format.")]
        public bool ScriptUpdate { get; set; }

        [Option('e', "exportResources", HelpText = "Export localization resources (either from database or from resource Xml files)")]
        public bool ExportResources { get; set; }

        [Option('f', "fromDatabase", HelpText = "Export localization resources from database. `EPiServerDB` connection string name will be used.")]
        public bool ExportFromDatabase { get; set; }

        [Option('x', "fromXmlOnly", HelpText = "Export localization resources only from XML files.")]
        public bool ExportFromXmlOnly { get; set; }

        [Option('j', "jsonFormat", HelpText = "Use JSON file format")]
        public bool Json { get; set; }

        [Option('i', "importResources", HelpText = "Import localization resources from SQL file into database")]
        public bool ImportResources { get; set; }

        [Option('d', "ignoreDuplicateKey", HelpText = "Ignore duplicate keys (Take first found)")]
        public bool IgnoreDuplicateKeys { get; set; }

        [Option('c', "connectionString", HelpText = "Sets connection string to be used for either reading or writing of the resources from/to database. This is for lazy (when reading value from web.config or app.config file is way too simple.)")]
        public string ConnectionString { get; set; }

        [Usage(ApplicationAlias = "DbLocalizationProvider.MigrationTool.exe")]
        public static IEnumerable<Example> Examples =>
            new List<Example>
            {
                new Example("Convert from EPiServer XML files to JSON file (getting ready for import)", new MigrationToolOptions { SourceDirectory = "c:\\project\\" })
            };
    }
}
