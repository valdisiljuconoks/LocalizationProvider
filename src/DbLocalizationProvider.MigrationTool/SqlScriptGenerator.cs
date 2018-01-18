using System;
using System.Collections.Generic;
using System.Text;

namespace DbLocalizationProvider.MigrationTool
{
    internal class SqlScriptGenerator
    {
        public string Generate(ICollection<LocalizationResource> resources, bool scriptUpdate = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @id INT;");

            foreach (var resourceEntry in resources)
            {
                var escapedResourceKey = resourceEntry.ResourceKey.Replace("'", "''");
                var insertStatement =$@"
    INSERT dbo.LocalizationResources VALUES (N'{escapedResourceKey}', '{resourceEntry.ModificationDate:yyyy-MM-dd HH:mm}', '{resourceEntry.Author}', 0, 0, 0);
    SET @id=IDENT_CURRENT('dbo.LocalizationResources');
";

                var updateStatement =
                    $@"
    UPDATE dbo.LocalizationResources SET ModificationDate = '{resourceEntry.ModificationDate:yyyy-MM-dd HH:mm}', Author = '{resourceEntry.Author}' WHERE ResourceKey = '{escapedResourceKey}';
    SELECT @id = id FROM dbo.LocalizationResources WHERE ResourceKey = '{escapedResourceKey}';
";

                var skipResourceStatement = $@"
    PRINT 'Skipping ""{ escapedResourceKey}"" because its already in the DB';
    SELECT @id = id FROM dbo.LocalizationResources WHERE ResourceKey = '{escapedResourceKey}';
";

                sb.Append($@"
IF EXISTS(SELECT 1 FROM dbo.LocalizationResources WHERE ResourceKey = '{escapedResourceKey}')
BEGIN
{(scriptUpdate ? updateStatement : skipResourceStatement)}
END
ELSE
BEGIN
{insertStatement}
END
");

                foreach (var resourceTranslation in resourceEntry.Translations)
                {
                    var translationInsertStatement = $@"
    INSERT dbo.LocalizationResourceTranslations (ResourceId, Language, Value) VALUES (@id, '{resourceTranslation.Language}', N'{resourceTranslation.Value.Replace("'", "''")}');
";

                    var translationUpdateStatement = $@"
    UPDATE dbo.LocalizationResourceTranslations SET VALUE = N'{resourceTranslation.Value.Replace("'", "''")}' WHERE ResourceId = @id AND [Language] = '{resourceTranslation.Language}';";

                    var skipTranslationStatement = $@"
    PRINT 'Skipping ""{ escapedResourceKey}"" for language ""{resourceTranslation.Language}"" because its already in the DB';
";

                    sb.Append($@"
IF EXISTS(SELECT 1 FROM dbo.LocalizationResourceTranslations WHERE ResourceId = @id AND [Language] = '{resourceTranslation.Language}')
BEGIN
{(scriptUpdate ? translationUpdateStatement : skipTranslationStatement)}
END
ELSE
BEGIN
{translationInsertStatement}
END");
                }
            }

            return sb.ToString();
        }
    }
}
