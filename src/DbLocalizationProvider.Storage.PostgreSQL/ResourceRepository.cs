// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    /// <summary>
    /// Repository for working with underlying MSSQL storage
    /// </summary>
    public class ResourceRepository
    {
        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns>List of resources</returns>
        public IEnumerable<LocalizationResource> GetAll()
        {
            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"SELECT
                        r.""Id"",
                        r.""ResourceKey"",
                        r.""Author"",
                        r.""FromCode"",
                        r.""IsHidden"",
                        r.""IsModified"",
                        r.""ModificationDate"",
                        r.""Notes"",
                        t.""Id"" as ""TranslationId"",
                        t.""Value"" as ""Translation"",
                        t.""Language"",
                        t.""ModificationDate"" as ""TranslationModificationDate""
                        FROM public.""LocalizationResources"" r
                    LEFT JOIN public.""LocalizationResourceTranslations"" t ON r.""Id"" = t.""ResourceId""",
                    conn);

                var reader = cmd.ExecuteReader();
                var lookup = new Dictionary<string, LocalizationResource>();

                void CreateTranslation(NpgsqlDataReader sqlDataReader, LocalizationResource localizationResource)
                {
                    if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("TranslationId")))
                    {
                        localizationResource.Translations.Add(new LocalizationResourceTranslation
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TranslationId")),
                            ResourceId = localizationResource.Id,
                            Value = sqlDataReader.GetStringSafe("Translation"),
                            Language = sqlDataReader.GetStringSafe("Language") ?? string.Empty,
                            ModificationDate = reader.GetDateTime(reader.GetOrdinal("TranslationModificationDate")),
                            LocalizationResource = localizationResource
                        });
                    }
                }

                while(reader.Read())
                {
                    var key = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.ResourceKey)));
                    if(lookup.TryGetValue(key, out var resource))
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
        }

        /// <summary>
        /// Gets resource by the key.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>Localized resource if found by given key</returns>
        /// <exception cref="ArgumentNullException">resourceKey</exception>
        public LocalizationResource GetByKey(string resourceKey)
        {
            if(resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                string strCmd = @"SELECT
                        r.""Id"",
                        r.""Author"",
                        r.""FromCode"",
                        r.""IsHidden"",
                        r.""IsModified"",
                        r.""ModificationDate"",
                        r.""Notes"",
                        t.""Id"" as ""TranslationId"",
                        t.""Value"" as ""Translation"",
                        t.""Language"",
                        t.""ModificationDate"" as ""TranslationModificationDate""
                    FROM public.""LocalizationResources"" r
                    LEFT JOIN public.""LocalizationResourceTranslations"" t ON r.""Id"" = t.""ResourceId""
                    WHERE ""ResourceKey"" = @Key;";

                var cmd = new NpgsqlCommand(strCmd, conn);
                cmd.Parameters.AddWithValue("Key", resourceKey);

                var reader = cmd.ExecuteReader();

                if(!reader.Read()) return null;

                var result = CreateResourceFromSqlReader(resourceKey, reader);

                // read 1st translation
                // if TranslationId is NULL - there is no translations for given resource
                if (!reader.IsDBNull(reader.GetOrdinal("TranslationId")))
                {
                    result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
                    while (reader.Read()) result.Translations.Add(CreateTranslationFromSqlReader(reader, result));
                }

                return result;
            }
        }

        private LocalizationResource CreateResourceFromSqlReader(string key, NpgsqlDataReader reader)
        {
            return new LocalizationResource(key)
            {
                Id = reader.GetInt32(reader.GetOrdinal(nameof(LocalizationResource.Id))),
                Author = reader.GetStringSafe(nameof(LocalizationResource.Author)) ?? "unknown",
                FromCode = reader.GetBooleanSafe(nameof(LocalizationResource.FromCode)),
                IsHidden = reader.GetBooleanSafe(nameof(LocalizationResource.IsHidden)),
                IsModified = reader.GetBooleanSafe(nameof(LocalizationResource.IsModified)),
                ModificationDate = reader.GetDateTime(reader.GetOrdinal(nameof(LocalizationResource.ModificationDate))),
                Notes = reader.GetStringSafe(nameof(LocalizationResource.Notes)),
            };
        }

        private LocalizationResourceTranslation CreateTranslationFromSqlReader(NpgsqlDataReader reader, LocalizationResource result)
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
            if(resource == null) throw new ArgumentNullException(nameof(resource));
            if(translation == null) throw new ArgumentNullException(nameof(translation));

            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"INSERT INTO public.""LocalizationResourceTranslations"" (""Language"", ""ResourceId"", ""Value"", ""ModificationDate"") VALUES (@language, @resourceId, @translation, @modificationDate)", conn);
                cmd.Parameters.AddWithValue("language", translation.Language);
                cmd.Parameters.AddWithValue("resourceId", translation.ResourceId);
                cmd.Parameters.AddWithValue("translation", translation.Value);
                cmd.Parameters.AddWithValue("modificationDate", translation.ModificationDate);

                cmd.ExecuteNonQuery();
            }
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
            if(resource == null) throw new ArgumentNullException(nameof(resource));
            if(translation == null) throw new ArgumentNullException(nameof(translation));

            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"UPDATE public.""LocalizationResourceTranslations"" SET ""Value"" = @translation, ""ModificationDate"" = @modificationDate WHERE ""Id"" = @id", conn);
                cmd.Parameters.AddWithValue("translation", translation.Value);
                cmd.Parameters.AddWithValue("id", translation.Id);
                cmd.Parameters.AddWithValue("modificationDate", translation.ModificationDate);

                cmd.ExecuteNonQuery();
            }
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
            if(resource == null) throw new ArgumentNullException(nameof(resource));
            if(translation == null) throw new ArgumentNullException(nameof(translation));

            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"DELETE FROM public.""LocalizationResourceTranslations"" WHERE ""Id"" = @id", conn);
                cmd.Parameters.AddWithValue("id", translation.Id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void UpdateResource(LocalizationResource resource)
        {
            if(resource == null) throw new ArgumentNullException(nameof(resource));

            using(var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"UPDATE public.""LocalizationResources"" SET ""IsModified"" = @isModified, ""ModificationDate"" = @modificationDate, ""Notes"" = @notes WHERE ""Id"" = @id", conn);
                cmd.Parameters.AddWithValue("id", resource.Id);
                cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);
                cmd.Parameters.AddWithValue("isModified", resource.IsModified);
                cmd.Parameters.AddWithValue("notes", (object)resource.Notes ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void DeleteResource(LocalizationResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"DELETE FROM public.""LocalizationResources"" WHERE ""Id"" = @id", conn);
                cmd.Parameters.AddWithValue("id", resource.Id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes all resources. DANGEROUS!
        /// </summary>
        public void DeleteAllResources()
        {
            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"DELETE FROM public.""LocalizationResources""", conn);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts the resource in database.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void InsertResource(LocalizationResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"INSERT INTO public.""LocalizationResources"" (""ResourceKey"", ""Author"", ""FromCode"", ""IsHidden"", ""IsModified"", ""ModificationDate"", ""Notes"") OUTPUT INSERTED.ID VALUES (@resourceKey, @author, @fromCode, @isHidden, @isModified, @modificationDate, @notes)", conn);

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
                        cmd = new NpgsqlCommand(@"INSERT INTO public.""LocalizationResourceTranslations"" (""Language"", ""ResourceId"", ""Value"", ""ModificationDate"") VALUES (@language, @resourceId, @translation, @modificationDate)", conn);
                        cmd.Parameters.AddWithValue("language", translation.Language);
                        cmd.Parameters.AddWithValue("resourceId", resourcePk);
                        cmd.Parameters.AddWithValue("translation", translation.Value);
                        cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the available languages (reads in which languages translations are added).
        /// </summary>
        /// <param name="includeInvariant">if set to <c>true</c> ""include invariant"".</param>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
        {
            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(@"SELECT DISTINCT ""Language"" FROM public.""LocalizationResourceTranslations"" WHERE ""Language"" <> ''", conn);
                var reader = cmd.ExecuteReader();

                var result = new List<CultureInfo>();
                if (includeInvariant) result.Add(CultureInfo.InvariantCulture);

                while (reader.Read())
                {
                    result.Add(new CultureInfo(reader.GetString(0)));
                }

                return result;
            }
        }
    }
}
