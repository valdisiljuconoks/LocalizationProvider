// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Data;
using System.Globalization;
using System.Text;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Storage.SqlServer;

/// <inheritdoc />
public class ResourceRepository : IResourceRepository
{
    private readonly bool _enableInvariantCultureFallback;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="configurationContext">Configuration settings.</param>
    public ResourceRepository(IOptions<ConfigurationContext> configurationContext)
    {
        _enableInvariantCultureFallback = configurationContext.Value.EnableInvariantCultureFallback;
        _logger = configurationContext.Value.Logger;
    }

    /// <inheritdoc />
    public IEnumerable<LocalizationResource> GetAll()
    {
        try
        {
            using var conn = new SqlConnection(Settings.DbContextConnectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                    SELECT
                        r.Id,
                        r.ResourceKey,
                        r.Author,
                        r.FromCode,
                        r.IsHidden,
                        r.IsModified,
                        r.ModificationDate,
                        r.Notes,
                        t.Id as TranslationId,
                        t.Value as Translation,
                        t.Language,
                        t.ModificationDate as TranslationModificationDate
                    FROM [dbo].[LocalizationResources] r
                    LEFT JOIN [dbo].[LocalizationResourceTranslations] t ON r.Id = t.ResourceId",
                                     conn);

            var reader = cmd.ExecuteReader();
            var lookup = new Dictionary<string, LocalizationResource>();

            void CreateTranslation(SqlDataReader sqlDataReader, LocalizationResource localizationResource)
            {
                if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("TranslationId")))
                {
                    localizationResource.Translations.Add(new LocalizationResourceTranslation
                    {
                        Id =
                            sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TranslationId")),
                        ResourceId = localizationResource.Id,
                        Value = sqlDataReader.GetStringSafe("Translation"),
                        Language = sqlDataReader.GetStringSafe("Language") ?? string.Empty,
                        ModificationDate = reader.GetDateTime(reader.GetOrdinal("TranslationModificationDate")),
                        LocalizationResource = localizationResource
                    });
                }
            }

            while (reader.Read())
            {
                var key = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.ResourceKey)));
                if (lookup.TryGetValue(key, out var resource))
                {
                    CreateTranslation(reader, resource);
                }
                else
                {
                    var result = CreateResourceFromSqlReader(key, reader);
                    CreateTranslation(reader, result);
                    lookup.Add(key, result);
                }
            }

            reader.Close();

            return lookup.Values;
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to retrieve all resources.", ex);
            return [];
        }
    }

    /// <inheritdoc />
    public LocalizationResource GetByKey(string resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        try
        {
            using var conn = new SqlConnection(Settings.DbContextConnectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                    SELECT
                        r.Id,
                        r.Author,
                        r.FromCode,
                        r.IsHidden,
                        r.IsModified,
                        r.ModificationDate,
                        r.Notes,
                        t.Id as TranslationId,
                        t.Value as Translation,
                        t.Language,
                        t.ModificationDate as TranslationModificationDate
                    FROM [dbo].[LocalizationResources] r
                    LEFT JOIN [dbo].[LocalizationResourceTranslations] t ON r.Id = t.ResourceId
                    WHERE ResourceKey = @key",
                                     conn);
            cmd.Parameters.AddWithValue("key", resourceKey);

            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            var result = CreateResourceFromSqlReader(resourceKey, reader);

            // read 1st translation
            // if TranslationId is NULL - there is no translations for given resource
            if (!reader.IsDBNull(reader.GetOrdinal("TranslationId")))
            {
                result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
                while (reader.Read())
                {
                    result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger?.Error($"Failed to retrieve resource by key {resourceKey}.", ex);
            return null;
        }
    }

    /// <inheritdoc />
    public void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(translation);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand(
            "INSERT INTO [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId], [Value], [ModificationDate]) VALUES (@language, @resourceId, @translation, @modificationDate)",
            conn);
        cmd.Parameters.AddWithValue("language", translation.Language);
        cmd.Parameters.AddWithValue("resourceId", translation.ResourceId);
        cmd.Parameters.AddWithValue("translation", translation.Value);
        cmd.Parameters.AddWithValue("modificationDate", translation.ModificationDate);

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(translation);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand(
            "UPDATE [dbo].[LocalizationResourceTranslations] SET [Value] = @translation, [ModificationDate] = @modificationDate WHERE [Id] = @id",
            conn);
        cmd.Parameters.AddWithValue("translation", translation.Value);
        cmd.Parameters.AddWithValue("id", translation.Id);
        cmd.Parameters.AddWithValue("modificationDate", DateTime.UtcNow);

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void DeleteTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(translation);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResourceTranslations] WHERE [Id] = @id", conn);
        cmd.Parameters.AddWithValue("id", translation.Id);

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void UpdateResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand(
            "UPDATE [dbo].[LocalizationResources] SET [IsModified] = @isModified, [ModificationDate] = @modificationDate, [Notes] = @notes WHERE [Id] = @id",
            conn);
        cmd.Parameters.AddWithValue("id", resource.Id);
        cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);
        cmd.Parameters.AddWithValue("isModified", resource.IsModified);
        cmd.Parameters.AddWithValue("notes", (object)resource.Notes ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void DeleteResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResources] WHERE [Id] = @id", conn);
        cmd.Parameters.AddWithValue("id", resource.Id);

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void DeleteAllResources()
    {
        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResourceTranslations]", conn);
        cmd.ExecuteNonQuery();

        cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResources]", conn);
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void InsertResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand(
            "INSERT INTO [dbo].[LocalizationResources] ([ResourceKey], [Author], [FromCode], [IsHidden], [IsModified], [ModificationDate], [Notes]) OUTPUT INSERTED.ID VALUES (@resourceKey, @author, @fromCode, @isHidden, @isModified, @modificationDate, @notes)",
            conn);

        cmd.Parameters.AddWithValue("resourceKey", resource.ResourceKey);
        cmd.Parameters.AddWithValue("author", resource.Author ?? "unknown");
        cmd.Parameters.AddWithValue("fromCode", resource.FromCode);
        cmd.Parameters.AddWithValue("isHidden", resource.IsHidden);
        cmd.Parameters.AddWithValue("isModified", resource.IsModified);
        cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);
        cmd.Parameters.AddSafeWithValue("notes", resource.Notes);

        // get inserted resource ID
        var resourcePk = (int)cmd.ExecuteScalar();

        // if there are also provided translations - execute those in the same connection also
        if (resource.Translations.Any())
        {
            foreach (var translation in resource.Translations)
            {
                cmd = new SqlCommand(
                    "INSERT INTO [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId], [Value], [ModificationDate]) VALUES (@language, @resourceId, @translation, @modificationDate)",
                    conn);
                cmd.Parameters.AddWithValue("language", translation.Language);
                cmd.Parameters.AddWithValue("resourceId", resourcePk);
                cmd.Parameters.AddWithValue("translation", translation.Value);
                cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);

                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
    {
        try
        {
            using var conn = new SqlConnection(Settings.DbContextConnectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT DISTINCT [Language] FROM [dbo].[LocalizationResourceTranslations] WHERE [Language] <> ''",
                conn);
            var reader = cmd.ExecuteReader();

            var result = new List<CultureInfo>();
            if (includeInvariant)
            {
                result.Add(CultureInfo.InvariantCulture);
            }

            while (reader.Read())
            {
                result.Add(CultureInfo.GetCultureInfo(reader.GetString(0)));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to retrieve all available languages.", ex);
            return [];
        }
    }

    /// <inheritdoc />
    public void ResetSyncStatus()
    {
        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        var cmd = new SqlCommand("UPDATE [dbo].[LocalizationResources] SET FromCode = 0", conn);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    /// <inheritdoc />
    public void RegisterDiscoveredResources(
        ICollection<DiscoveredResource> discoveredResources,
        Dictionary<string, LocalizationResource>? allResources,
        bool flexibleRefactoringMode,
        SyncSource source)
    {
        var group = discoveredResources;

        var sb = new StringBuilder();
        sb.AppendLine("DECLARE @resourceId INT");

        var allNewResources = discoveredResources.Where(dr => !allResources.ContainsKey(dr.Key)).ToList();
        var allMatchingResources = discoveredResources.Except(allNewResources).ToList();

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        // handle matching - we need to update existing resources
        if (allMatchingResources.Count > 0)
        {
            var matchingResourcesTable = new DataTable();
            matchingResourcesTable.Columns.Add("ResourceKey", typeof(string));
            matchingResourcesTable.Columns.Add("FromCode", typeof(bool));
            matchingResourcesTable.Columns.Add("IsHidden", typeof(bool));

            foreach (var x in allMatchingResources)
            {
                matchingResourcesTable.Rows.Add(x.Key, true, x.IsHidden);
            }

            var cmdTmp = new SqlCommand(
                $"CREATE TABLE #TempTable{source} (ResourceKey nvarchar(800), FromCode bit, IsHidden bit)",
                conn) { CommandTimeout = 60 };

            cmdTmp.ExecuteNonQuery();

            using var bulkCopy = new SqlBulkCopy(conn);

            bulkCopy.DestinationTableName = $"#TempTable{source}";
            bulkCopy.ColumnMappings.Add("ResourceKey", "ResourceKey");
            bulkCopy.ColumnMappings.Add("FromCode", "FromCode");
            bulkCopy.ColumnMappings.Add("IsHidden", "IsHidden");

            // Write data to the server
            bulkCopy.WriteToServer(matchingResourcesTable);

            cmdTmp.CommandText = @$"
MERGE INTO [LocalizationResources] AS Target
    USING #TempTable{source} AS Source
    ON Target.ResourceKey = Source.ResourceKey
    WHEN MATCHED THEN
        UPDATE SET
            Target.FromCode = Source.FromCode,
            Target.IsHidden = Source.IsHidden;

DROP TABLE #TempTable{source}
";
            cmdTmp.ExecuteNonQuery();
        }

        var refactoredResources = group.Where(r => !string.IsNullOrEmpty(r.OldResourceKey));
        foreach (var refactoredResource in refactoredResources)
        {
            sb.Append($@"
        IF EXISTS(SELECT 1 FROM LocalizationResources WITH(NOLOCK) WHERE ResourceKey = '{refactoredResource.OldResourceKey}'{(flexibleRefactoringMode ? " AND NOT EXISTS(SELECT 1 FROM LocalizationResources WITH(NOLOCK) WHERE ResourceKey = '" + refactoredResource.Key + "')" : string.Empty)})
        BEGIN
            UPDATE dbo.LocalizationResources SET ResourceKey = '{refactoredResource.Key}', FromCode = 1 WHERE ResourceKey = '{refactoredResource.OldResourceKey}'
        END
        ");
        }

        // handle all new resources - we need to insert
        if (allNewResources.Count > 0)
        {
            var newResourcesTable = new DataTable();
            newResourcesTable.Columns.Add("ResourceKey", typeof(string));
            newResourcesTable.Columns.Add("ModificationDate", typeof(DateTime));
            newResourcesTable.Columns.Add("Author", typeof(string));
            newResourcesTable.Columns.Add("FromCode", typeof(bool));
            newResourcesTable.Columns.Add("IsModified", typeof(bool));
            newResourcesTable.Columns.Add("IsHidden", typeof(bool));

            foreach (var newResource in allNewResources)
            {
                newResourcesTable.Rows.Add(
                    newResource.Key,
                    DateTime.UtcNow,
                    "type-scanner",
                    true,
                    false,
                    newResource.IsHidden);
            }

            using var newResourceBulkCopy = new SqlBulkCopy(conn);

            newResourceBulkCopy.DestinationTableName = "LocalizationResources";

            newResourceBulkCopy.ColumnMappings.Add("ResourceKey", "ResourceKey");
            newResourceBulkCopy.ColumnMappings.Add("ModificationDate", "ModificationDate");
            newResourceBulkCopy.ColumnMappings.Add("Author", "Author");
            newResourceBulkCopy.ColumnMappings.Add("FromCode", "FromCode");
            newResourceBulkCopy.ColumnMappings.Add("IsModified", "IsModified");
            newResourceBulkCopy.ColumnMappings.Add("IsHidden", "IsHidden");

            newResourceBulkCopy.WriteToServer(newResourcesTable);

            var newTranslationsTable = new DataTable();
            newTranslationsTable.Columns.Add("ResourceId", typeof(string));
            newTranslationsTable.Columns.Add("Language", typeof(string));
            newTranslationsTable.Columns.Add("Value", typeof(string));
            newTranslationsTable.Columns.Add("ModificationDate", typeof(DateTime));

            var idLookupCmd = new SqlCommand("SELECT Id, ResourceKey FROM LocalizationResources", conn);
            var idReader = idLookupCmd.ExecuteReader();

            while (idReader.Read())
            {
                var newResource = allNewResources.FirstOrDefault(x => x.Key.Equals(idReader.GetString(1), StringComparison.Ordinal));
                if (newResource == null)
                {
                    continue;
                }

                var id = idReader.GetInt32(0);

                foreach (var newTranslation in newResource.Translations)
                {
                    newTranslationsTable.Rows.Add(
                        id,
                        newTranslation.Culture,
                        newTranslation.Translation.Replace("'", "''"),
                        DateTime.UtcNow);
                }
            }

            idReader.Close();

            if (newTranslationsTable.Rows.Count > 0)
            {
                using var newTranslationsBulkCopy = new SqlBulkCopy(conn);
                newTranslationsBulkCopy.DestinationTableName = "LocalizationResourceTranslations";

                newTranslationsBulkCopy.ColumnMappings.Add("ResourceId", "ResourceId");
                newTranslationsBulkCopy.ColumnMappings.Add("Language", "Language");
                newTranslationsBulkCopy.ColumnMappings.Add("Value", "Value");
                newTranslationsBulkCopy.ColumnMappings.Add("ModificationDate", "ModificationDate");

                newTranslationsBulkCopy.WriteToServer(newTranslationsTable);
            }
        }

        foreach (var property in group)
        {
            if (!allResources.TryGetValue(property.Key, out var existingResource))
            {
                continue;
            }

            var invariantTranslation = property.Translations.InvariantTranslation();
            if (invariantTranslation != null)
            {
                var existingInvariant = existingResource.Translations.InvariantTranslation();

                if (existingInvariant?.Value == null || !string.Equals(invariantTranslation.Translation, existingInvariant.Value, StringComparison.InvariantCulture))
                {
                    sb.AppendLine(
                        $"UPDATE [LocalizationResourceTranslations] SET [Value] = N'{invariantTranslation.Translation.Replace("'", "''")}' WHERE [ResourceId]={existingResource.Id} AND [Language]='{invariantTranslation.Culture}'");
                }
            }

            if (!existingResource.IsModified.HasValue || existingResource.IsModified.Value)
            {
                continue;
            }

            foreach (var propertyTranslation in property.Translations)
            {
                AddTranslationScript(existingResource, sb, propertyTranslation);
            }
        }

        var cmd = new SqlCommand(sb.ToString(), conn) { CommandTimeout = 60 };

        cmd.ExecuteNonQuery();
        conn.Close();
    }

    private LocalizationResource CreateResourceFromSqlReader(string key, SqlDataReader reader)
    {
        return new LocalizationResource(key, _enableInvariantCultureFallback)
        {
            Id = reader.GetInt32(reader.GetOrdinal(nameof(LocalizationResource.Id))),
            Author = reader.GetStringSafe(nameof(LocalizationResource.Author)) ?? "unknown",
            FromCode = reader.GetBooleanSafe(nameof(LocalizationResource.FromCode)),
            IsHidden = reader.GetBooleanSafe(nameof(LocalizationResource.IsHidden)),
            IsModified = reader.GetBooleanSafe(nameof(LocalizationResource.IsModified)),
            ModificationDate = reader.GetDateTime(reader.GetOrdinal(nameof(LocalizationResource.ModificationDate))),
            Notes = reader.GetStringSafe(nameof(LocalizationResource.Notes))
        };
    }

    private LocalizationResourceTranslation CreateTranslationFromSqlReader(SqlDataReader reader, LocalizationResource result)
    {
        return new LocalizationResourceTranslation
        {
            Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
            ResourceId = result.Id,
            Value = reader.GetStringSafe("Translation"),
            Language = reader.GetStringSafe("Language") ?? string.Empty,
            ModificationDate = reader.GetDateTime(reader.GetOrdinal("TranslationModificationDate")),
            LocalizationResource = result
        };
    }

    private static void AddTranslationScript(
        LocalizationResource existingResource,
        StringBuilder buffer,
        DiscoveredTranslation resource)
    {
        var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == resource.Culture);
        if (existingTranslation == null)
        {
            buffer.Append($@"
        INSERT INTO [LocalizationResourceTranslations] ([ResourceId], [Language], [Value], [ModificationDate]) VALUES ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}', GETUTCDATE())");
        }
        else if (existingTranslation.Value == null || !existingTranslation.Value.Equals(resource.Translation))
        {
            buffer.Append($@"
        UPDATE [LocalizationResourceTranslations] SET [Value] = N'{resource.Translation.Replace("'", "''")}' WHERE [ResourceId]={existingResource.Id} and [Language]='{resource.Culture}'");
        }
    }
}
