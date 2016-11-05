using NDesk.Options;

namespace DbLocalizationProvider.MigrationTool
{
    internal class MigrationToolSettings
    {
        public OptionSet OptionSet { get; private set; }

        public MigrationToolSettings(OptionSet optionSet)
        {
            OptionSet = optionSet;
        }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public bool ScriptUpdate { get; set; }
        public bool ExportResources { get; set; }
        public bool ImportResources { get; set; }
        public bool ShowHelp { get; set; }
        public string ConnectionString { get; set; }
        public bool ExportFromDatabase { get; set; }
        public bool Json { get; set; }
        public string ResourceDirectory { get; set; }
    }
}
