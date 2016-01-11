using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TechFellow.DbLocalizationProvider;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    internal class ResourceImporter
    {
        public void Import(MigrationToolSettings settings)
        {
            var sourceImportFilePath = Path.Combine(settings.SourceDirectory, "localization-resource-translations.sql");
            if (!File.Exists(sourceImportFilePath))
            {
                throw new IOException($"Source file '{sourceImportFilePath}' for import not found!");
            }

            // create DB structures in target database
            using (var db = new LanguageEntities(settings.ConnectionString))
            {
                var resource = db.LocalizationResources.Where(r => r.Id == 0);
            }

            var fileInfo = new FileInfo(sourceImportFilePath);
            var script = fileInfo.OpenText().ReadToEnd();
            using (var connection = new SqlConnection(settings.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
