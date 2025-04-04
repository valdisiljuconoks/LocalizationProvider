// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Azure.Data.Tables;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Storage.AzureTables;

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
            var table = GetTableClient();
            var result = table.ExecuteQuery(CreateResourcesByPartitionFilter());

            return result.Select(FromEntity).ToList();
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to retrieve all resources.", ex);
            return [];
        }
    }

    /// <inheritdoc />
    public LocalizationResource? GetByKey(string resourceKey)
    {
        ArgumentNullException.ThrowIfNull(resourceKey);

        try
        {
            var entity = GetEntityByKey(resourceKey);
            return FromEntity(entity);
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

        resource.Translations.Add(translation);

        var client = GetTableClient();
        var entity = new LocalizationResourceEntity(resource.ResourceKey);

        Map(resource, entity);
        client.UpsertEntity(entity);
    }


    /// <inheritdoc />
    public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(translation);

        Save(resource);
    }

    /// <inheritdoc />
    public void DeleteTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(translation);

        resource.Translations.Remove(resource.Translations.FindByLanguage(translation.Language));

        Save(resource);
    }

    /// <inheritdoc />
    public void UpdateResource(LocalizationResource resource)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        Save(resource);
    }

    /// <inheritdoc />
    public void DeleteResource(LocalizationResource resource)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var table = GetTableClient();
        DeleteEntity(LocalizationResourceEntity.PartitionKeyValue, resource.ResourceKey, table);
    }


    /// <inheritdoc />
    public void DeleteAllResources()
    {
        var table = GetTableClient();
        var allResources = GetAll();

        foreach (var key in allResources.Select(r => r.ResourceKey))
        {
            DeleteEntity(LocalizationResourceEntity.PartitionKeyValue, key, table);
        }
    }

    /// <inheritdoc />
    public void InsertResource(LocalizationResource resource)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var table = GetTableClient();
        var entity = new LocalizationResourceEntity(resource.ResourceKey);
        Map(resource, entity);
        table.AddEntity(entity);
    }

    /// <inheritdoc />
    public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
    {
        try
        {
            var allResources = GetAll();

            return allResources
                .SelectMany(r => r.Translations.Select(t => t.Language))
                .Distinct()
                .Where(l => includeInvariant || l != string.Empty)
                .Select(CultureInfo.GetCultureInfo);
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
        var allResources = GetAll();
        var allKeys = allResources.Select(r => r.ResourceKey);
        var table = GetTableClient();

        foreach (var key in allKeys)
        {
            var entity = GetEntityByKey(key);
            entity.FromCode = false;
            table.UpsertEntity(entity);
        }
    }

    /// <inheritdoc />
    public void RegisterDiscoveredResources(
        ICollection<DiscoveredResource> discoveredResources,
        Dictionary<string, LocalizationResource>? allResources,
        bool flexibleRefactoringMode,
        SyncSource source)
    {
        foreach (var discoveredResource in discoveredResources)
        {
            if (!allResources.TryGetValue(discoveredResource.Key, out var existingResource))
            {
                InsertResource(ToResource(discoveredResource));
            }

            if (existingResource == null)
            {
                continue;
            }

            if (existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
            {
                existingResource.FromCode = true;
                existingResource.IsHidden = discoveredResource.IsHidden;

                foreach (var translation in discoveredResource.Translations)
                {
                    var existingTranslation = existingResource.Translations.FindByLanguage(translation.Culture);
                    if (existingTranslation == null)
                    {
                        existingResource.Translations.Add(new LocalizationResourceTranslation
                        {
                            Language = translation.Culture,
                            ModificationDate = DateTime.UtcNow,
                            Value = translation.Translation
                        });
                    }
                    else
                    {
                        existingTranslation.ModificationDate = DateTime.UtcNow;
                        existingTranslation.Value = translation.Translation;
                    }
                }
            }

            Save(existingResource);
        }
    }

    private static Expression<Func<LocalizationResourceEntity, bool>> CreateResourcesByPartitionFilter()
    {
        return e => e.PartitionKey == LocalizationResourceEntity.PartitionKeyValue;
    }

    private static LocalizationResourceEntity GetEntityByKey(string resourceKey)
    {
        var table = GetTableClient();
        return table.GetEntity<LocalizationResourceEntity>(LocalizationResourceEntity.PartitionKeyValue, resourceKey);
    }

    private void Save(LocalizationResource resource)
    {
        var table = GetTableClient();
        var entity = GetEntityByKey(resource.ResourceKey);
        Map(resource, entity);
        table.UpsertEntity(entity);
    }

    private static void DeleteEntity(string partitionKey, string rowKey, TableClient table)
    {
        table.DeleteEntity(partitionKey, rowKey);
    }

    private static TableClient GetTableClient()
    {
        return new TableClient(Settings.ConnectionString, "LocalizationResources");
    }

    private LocalizationResource ToResource(DiscoveredResource discoveredResource)
    {
        var resource = new LocalizationResource(discoveredResource.Key, true)
        {
            Author = "type-scanner",
            ModificationDate = DateTime.UtcNow,
            FromCode = true,
            IsModified = false,
            IsHidden = discoveredResource.IsHidden
        };

        resource.Translations.AddRange(discoveredResource.Translations.Select(ToTranslation));
        return resource;
    }

    private static LocalizationResourceTranslation ToTranslation(DiscoveredTranslation translation)
    {
        return new LocalizationResourceTranslation
        {
            Language = translation.Culture, Value = translation.Translation, ModificationDate = DateTime.UtcNow
        };
    }

    private void Map(LocalizationResource resource, LocalizationResourceEntity entity)
    {
        entity.Author = resource.Author;
        entity.ModificationDate = DateTime.UtcNow;
        entity.FromCode = resource.FromCode;
        entity.IsModified = resource.IsModified ?? true;
        entity.IsHidden = resource.IsHidden ?? false;
        entity.Translations = JsonConvert.SerializeObject(resource.Translations.Select(ToTranslationEntity).ToList());
    }

    private LocalizationResourceTranslationEntity ToTranslationEntity(LocalizationResourceTranslation translation)
    {
        return new LocalizationResourceTranslationEntity
        {
            Language = translation.Language, Translation = translation.Value, ModificationDate = DateTime.UtcNow
        };
    }

    private LocalizationResource? FromEntity(LocalizationResourceEntity? firstOrDefault)
    {
        if (firstOrDefault == null)
        {
            return null;
        }

        var result = new LocalizationResource(firstOrDefault.RowKey, _enableInvariantCultureFallback)
        {
            Author = firstOrDefault.Author,
            ModificationDate = firstOrDefault.ModificationDate,
            FromCode = firstOrDefault.FromCode,
            IsModified = firstOrDefault.IsModified,
            IsHidden = firstOrDefault.IsHidden
        };

        var translationEntities =
            JsonConvert.DeserializeObject<LocalizationResourceTranslationEntity[]>(firstOrDefault.Translations);

        if (translationEntities?.Length > 0)
        {
            result.Translations.AddRange(translationEntities.Select(te => FromTranslationEntity(te, result)));
        }

        return result;
    }

    private LocalizationResourceTranslation FromTranslationEntity(
        LocalizationResourceTranslationEntity translationEntity,
        LocalizationResource localizationResource)
    {
        return new LocalizationResourceTranslation
        {
            Language = translationEntity.Language,
            Value = translationEntity.Translation,
            ModificationDate = translationEntity.ModificationDate,
            LocalizationResource = localizationResource
        };
    }
}
