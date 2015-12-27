using System.IO;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class ScriptFileWriter
    {
        public void Write(string generatedScript, string targetDirectory)
        {
            // clear previous state (if any)
            if (File.Exists(Path.Combine(targetDirectory, "localization-resource-translations.sql")))
            {
                File.Delete(Path.Combine(targetDirectory, "localization-resource-translations.sql"));
            }

            using (var outputFile = File.Open(Path.Combine(targetDirectory, "localization-resource-translations.sql"), FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(outputFile))
                {
                    writer.Write(generatedScript);
                }
            }
        }
    }
}
