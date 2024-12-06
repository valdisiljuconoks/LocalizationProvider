// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Storage.SqlServer;

/// <summary>
/// Repository for working with underlying MSSQL storage
/// </summary>
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

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <returns>List of resources</returns>
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

            return lookup.Values;
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to retrieve all resources.", ex);
            return [];
        }
    }

    /// <summary>
    /// Gets resource by the key.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>Localized resource if found by given key</returns>
    /// <exception cref="ArgumentNullException">resourceKey</exception>
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

    /// <summary>
    /// Adds the translation for the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="translation">The translation.</param>
    /// <exception cref="ArgumentNullException">
    /// resource
    /// or
    /// translation
    /// </exception>
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

    /// <summary>
    /// Updates the translation for the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="translation">The translation.</param>
    /// <exception cref="ArgumentNullException">
    /// resource
    /// or
    /// translation
    /// </exception>
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

    /// <summary>
    /// Deletes the translation.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="translation">The translation.</param>
    /// <exception cref="ArgumentNullException">
    /// resource
    /// or
    /// translation
    /// </exception>
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

    /// <summary>
    /// Updates the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
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

    /// <summary>
    /// Deletes the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    public void DeleteResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResources] WHERE [Id] = @id", conn);
        cmd.Parameters.AddWithValue("id", resource.Id);

        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes all resources. DANGEROUS!
    /// </summary>
    public void DeleteAllResources()
    {
        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResourceTranslations]", conn);
        cmd.ExecuteNonQuery();

        cmd = new SqlCommand("DELETE FROM [dbo].[LocalizationResources]", conn);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts the resource in database.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
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

    /// <summary>
    /// Gets the available languages (reads in which languages translations are added).
    /// </summary>
    /// <param name="includeInvariant">if set to <c>true</c> [include invariant].</param>
    /// <returns></returns>
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

    /// <summary>
    /// Resets synchronization status of the resources.
    /// </summary>
    public void ResetSyncStatus()
    {
        using var conn = new SqlConnection(Settings.DbContextConnectionString);
        var cmd = new SqlCommand("UPDATE [dbo].[LocalizationResources] SET FromCode = 0", conn);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    /// <summary>
    /// Registers discovered resources.
    /// </summary>
    /// <param name="discoveredResources">Collection of discovered resources during scanning process.</param>
    /// <param name="allResources">All existing resources (so you could compare and decide what script to generate).</param>
    /// <param name="flexibleRefactoringMode">Run refactored resource sync in flexible / relaxed mode (leave existing resources in db).</param>
    public void RegisterDiscoveredResources(
        ICollection<DiscoveredResource> discoveredResources,
        Dictionary<string, LocalizationResource> allResources,
        bool flexibleRefactoringMode)
    {
        // split work queue by 400 resources each
        var groupedProperties = discoveredResources.SplitByCount(400);

        Parallel.ForEach(
            groupedProperties,
            group =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("DECLARE @resourceId INT");

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

                foreach (var property in group)
                {
                    if (!allResources.TryGetValue(property.Key, out var existingResource))
                    {
                        sb.Append($@"
        SET @resourceId = ISNULL((SELECT Id FROM LocalizationResources WHERE [ResourceKey] = '{property.Key}'), -1)
        IF (@resourceId = -1)
        BEGIN
            INSERT INTO LocalizationResources ([ResourceKey], ModificationDate, Author, FromCode, IsModified, IsHidden)
            VALUES ('{property.Key}', GETUTCDATE(), 'type-scanner', 1, 0, {Convert.ToInt32(property.IsHidden)})
            SET @resourceId = SCOPE_IDENTITY()");

                        // add all translations
                        foreach (var propertyTranslation in property.Translations)
                        {
                            sb.Append($@"
            INSERT INTO LocalizationResourceTranslations (ResourceId, [Language], [Value], [ModificationDate]) VALUES (@resourceId, '{propertyTranslation.Culture}', N'{propertyTranslation.Translation.Replace("'", "''")}', GETUTCDATE())");
                        }

                        sb.Append(@"
        END
        ");
                    }

                    if (existingResource != null)
                    {
                        sb.AppendLine(
                            $"UPDATE LocalizationResources SET FromCode = 1, IsHidden = {Convert.ToInt32(property.IsHidden)} where [Id] = {existingResource.Id}");

                        var invariantTranslation = property.Translations.FirstOrDefault(t => t.Culture == string.Empty);
                        if (invariantTranslation != null)
                        {
                            sb.AppendLine(
                                $"UPDATE LocalizationResourceTranslations SET [Value] = N'{invariantTranslation.Translation.Replace("'", "''")}' where ResourceId={existingResource.Id} AND [Language]='{invariantTranslation.Culture}'");
                        }

                        if (existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                        {
                            foreach (var propertyTranslation in property.Translations)
                            {
                                AddTranslationScript(existingResource, sb, propertyTranslation);
                            }
                        }
                    }
                }

                using var conn = new SqlConnection(Settings.DbContextConnectionString);
                var cmd = new SqlCommand(sb.ToString(), conn) { CommandTimeout = 60 };

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            });
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
        INSERT INTO [dbo].[LocalizationResourceTranslations] (ResourceId, [Language], [Value], [ModificationDate]) VALUES ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}', GETUTCDATE())");
        }
        else if (existingTranslation.Value == null || !existingTranslation.Value.Equals(resource.Translation))
        {
            buffer.Append($@"
        UPDATE [dbo].[LocalizationResourceTranslations] SET [Value] = N'{resource.Translation.Replace("'", "''")}' WHERE ResourceId={existingResource.Id} and [Language]='{resource.Culture}'");
        }
    }
}
