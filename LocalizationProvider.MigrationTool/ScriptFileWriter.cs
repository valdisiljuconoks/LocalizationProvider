using System.IO;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class ScriptFileWriter
    {
        public string Write(string generatedScript, string targetDirectory)
        {
            // clear previous state (if any)
            var outputFilePath = Path.Combine(targetDirectory, "localization-resource-translations.sql");
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            using (var outputFile = File.Open(outputFilePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(outputFile))
                {
                    writer.Write(generatedScript);
                }
            }

            return outputFilePath;
        }
    }
}
