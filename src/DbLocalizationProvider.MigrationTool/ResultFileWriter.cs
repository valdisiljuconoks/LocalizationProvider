using System.IO;
using System.Text;

namespace DbLocalizationProvider.MigrationTool
{
    public class ResultFileWriter
    {
        public string Write(string generatedScript, string targetDirectory, bool json)
        {
            // clear previous state (if any)
            var outputFilePath = Path.Combine(targetDirectory, "localization-resource-translations." + (json ? "json" : "sql"));
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            using (var outputFile = File.Open(outputFilePath, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(outputFile, Encoding.UTF8))
                {
                    writer.Write(generatedScript);
                }
            }

            return outputFilePath;
        }
    }
}
