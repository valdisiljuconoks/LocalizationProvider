using System;
using System.Collections.Generic;
using System.Text;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class ScriptGenerator
    {
        public string Generate(ICollection<ResourceEntry> resources, bool scriptUpdate = false)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var sb = new StringBuilder();

            sb.AppendLine("DECLARE @id INT;");

            foreach (var resourceEntry in resources)
            {
                var insertStatement =
                    $@"
INSERT dbo.LocalizationResources VALUES (N'{resourceEntry.Key.Replace("'", "''")
                        }', getdate(), 'migration-tool');
SET @id=IDENT_CURRENT('dbo.LocalizationResources');";

                var updateStatement =
                    $@"
UPDATE dbo.LocalizationResources SET ModificationDate = getdate(), Author = 'migration-tool' WHERE ResourceKey = '{resourceEntry.Key.Replace("'", "''")
                        }';
SELECT @id = id FROM dbo.LocalizationResources WHERE ResourceKey = '{resourceEntry.Key.Replace("'", "''")}';";

                if (scriptUpdate)
                {
                    sb.Append(
                              $@"
IF EXISTS(SELECT 1 FROM dbo.LocalizationResources WHERE ResourceKey = '{resourceEntry.Key.Replace("'", "''")}')
BEGIN
    {updateStatement
                                  }
END
ELSE
BEGIN
    {insertStatement}
END");
                }
                else
                {
                    sb.Append(insertStatement);
                }

                foreach (var resourceTranslation in resourceEntry.Translations)
                {
                    var translationInsertStatement =
                        $@"
INSERT dbo.LocalizationResourceTranslations (ResourceId, Language, Value) VALUES (@id, '{resourceTranslation.CultureId}', N'{
                            resourceTranslation.Translation.Replace("'", "''")}');";

                    var translationUpdateStatement =
                        $@"
UPDATE dbo.LocalizationResourceTranslations SET VALUE = N'{resourceTranslation.Translation.Replace("'", "''")
                            }' WHERE ResourceId = @id AND [Language] = '{resourceTranslation.CultureId}';";

                    if (scriptUpdate)
                    {
                        sb.Append(
                                  $@"
IF EXISTS(SELECT 1 FROM dbo.LocalizationResourceTranslations WHERE ResourceId = @id AND [Language] = '{resourceTranslation.CultureId
                                      }')
BEGIN
    {translationUpdateStatement}
END
ELSE
BEGIN
    {translationInsertStatement}
END");
                    }
                    else
                    {
                        sb.Append(translationInsertStatement);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
