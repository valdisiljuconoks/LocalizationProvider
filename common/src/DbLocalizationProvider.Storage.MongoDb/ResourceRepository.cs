// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DbLocalizationProvider.Storage.MongoDb;

public class ResourceRepository(
    IOptions<ConfigurationContext> configurationContext,
    CounterRepository counterRepository,
    CollectionProvider collectionProvider)
    : IResourceRepository
{
    private const string CollectionName = "Resources";

    private readonly IMongoCollection<ResourceRecord> _collection =
        collectionProvider.GetCollection<ResourceRecord>(CollectionName);

    private readonly bool _enableInvariantCultureFallback = configurationContext.Value.EnableInvariantCultureFallback;
    private readonly ILogger _logger = configurationContext.Value.Logger;

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <returns>List of resources</returns>
    public IEnumerable<LocalizationResource> GetAll()
    {
        try
        {
            return _collection
                .Find(rr => true)
                .ToList()
                .Select(CreateResourceFromRecord);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to retrieve all resources.", ex);
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
            var rr = _collection
                .Find(r => r.ResourceKey == resourceKey)
                .FirstOrDefault();

            return rr == null ? null! : CreateResourceFromRecord(rr);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to retrieve resource by key {resourceKey}.", ex);
            return null!;
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

        var filter = Builders<ResourceRecord>.Filter.Eq(x => x.Id, resource.Id);
        var tr = new TranslationRecord
        {
            Id = NewTranslationId(),
            Value = translation.Value,
            Language = translation.Language,
            ModificationDate = translation.ModificationDate
        };
        _collection.UpdateOne(filter, Builders<ResourceRecord>.Update.Push(x => x.Translations, tr));
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

        var filter = Builders<ResourceRecord>.Filter.And(
            Builders<ResourceRecord>.Filter.Eq(x => x.Id, resource.Id),
            Builders<ResourceRecord>.Filter.ElemMatch(x => x.Translations, t => t.Id == translation.Id));

        var update = Builders<ResourceRecord>.Update
            .Set(r => r.Translations.FirstMatchingElement().Value, translation.Value)
            .Set(r => r.Translations.FirstMatchingElement().ModificationDate, DateTime.UtcNow);

        _collection.UpdateOne(filter, update);
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

        _collection.UpdateOne(
            Builders<ResourceRecord>.Filter.Eq(x => x.Id, resource.Id),
            Builders<ResourceRecord>.Update.PullFilter(x => x.Translations, t => t.Id == translation.Id));
    }

    /// <summary>
    /// Updates the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    public void UpdateResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        _collection.UpdateOne(
            Builders<ResourceRecord>.Filter.Eq(x => x.Id, resource.Id),
            Builders<ResourceRecord>.Update
                .Set(x => x.IsModified, resource.IsModified)
                .Set(x => x.ModificationDate, resource.ModificationDate)
                .Set(x => x.Notes, resource.Notes));
    }

    /// <summary>
    /// Deletes the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    public void DeleteResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        _collection.DeleteOne(x => x.Id == resource.Id);
    }

    /// <summary>
    /// Deletes all resources. DANGEROUS!
    /// </summary>
    public void DeleteAllResources()
    {
        _collection.DeleteMany(FilterDefinition<ResourceRecord>.Empty);
    }

    /// <summary>
    /// Inserts the resource in database.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    public void InsertResource(LocalizationResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var rr = new ResourceRecord
        {
            Id = NewResourceId(),
            ResourceKey = resource.ResourceKey,
            Author = resource.Author,
            FromCode = resource.FromCode,
            IsModified = resource.IsModified,
            IsHidden = resource.IsHidden,
            ModificationDate = resource.ModificationDate,
            Notes = resource.Notes,
            Translations = resource.Translations
                .Select(x => new TranslationRecord
                {
                    Id = NewTranslationId(),
                    Value = x.Value,
                    Language = x.Language,
                    ModificationDate = x.ModificationDate
                })
                .ToList()
        };

        _collection.InsertOne(rr);
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
            var languages = _collection
                .Distinct(
                    x => x.Translations.Select(t => t.Language),
                    FilterDefinition<ResourceRecord>.Empty
                )
                .ToList()
                .SelectMany(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new CultureInfo(x));

            if (includeInvariant)
            {
                return new[] { CultureInfo.InvariantCulture }
                    .Concat(languages);
            }

            return languages;
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to retrieve all available languages.", ex);
            return [];
        }
    }

    /// <summary>
    /// Resets synchronization status of the resources.
    /// </summary>
    public void ResetSyncStatus()
    {
        _collection.UpdateMany(
            FilterDefinition<ResourceRecord>.Empty,
            Builders<ResourceRecord>.Update.Set(x => x.FromCode, false));
    }

    /// <summary>
    /// Performs refactoring of keys and synchronization of discovered resources in a single bulk operation.
    /// </summary>
    /// <param name="discoveredResources">Resources discovered during scanning.</param>
    /// <param name="allResources">Existing resources for comparison.</param>
    /// <param name="flexibleRefactoringMode">If true, renames only when new key does not exist.</param>
    public void RegisterDiscoveredResources(
        ICollection<DiscoveredResource> discoveredResources,
        IEnumerable<LocalizationResource> allResources,
        bool flexibleRefactoringMode)
    {
        // Initialize mutable snapshot from SQL state
        var snapshot = CreateSnapshot(allResources);

        // Build and apply in-memory renames
        var refactorOps = BuildRefactorOps(snapshot, discoveredResources, flexibleRefactoringMode);

        // Build sync operations against updated snapshot
        var syncOps = BuildSyncOps(snapshot, discoveredResources);

        // Execute all collected operations in one bulk write
        var allOps = refactorOps.Concat(syncOps);
        ExecuteBulkWrite(allOps);
    }

    private List<LocalizationResource> CreateSnapshot(IEnumerable<LocalizationResource> allResources)
    {
        // Deep-copy snapshot to avoid mutating original
        return allResources
            .Select(r =>
            {
                var resource = new LocalizationResource
                {
                    Id = r.Id,
                    ResourceKey = r.ResourceKey,
                    Author = r.Author,
                    FromCode = r.FromCode,
                    IsModified = r.IsModified,
                    IsHidden = r.IsHidden,
                    ModificationDate = r.ModificationDate,
                    Notes = r.Notes
                };

                var translations = r.Translations
                    .Select(t => new LocalizationResourceTranslation
                    {
                        Id = t.Id,
                        ResourceId = t.ResourceId,
                        Language = t.Language,
                        Value = t.Value,
                        ModificationDate = t.ModificationDate
                    })
                    .ToList();
                resource.Translations.AddRange(translations);

                return resource;
            })
            .ToList();
    }

    private List<WriteModel<ResourceRecord>> BuildRefactorOps(
        List<LocalizationResource> snapshot,
        IEnumerable<DiscoveredResource> discoveredResources,
        bool flexible)
    {
        var ops = new List<WriteModel<ResourceRecord>>();
        var originalKeys = new HashSet<string>(snapshot.Select(r => r.ResourceKey));

        foreach (var d in discoveredResources.Where(r => !string.IsNullOrEmpty(r.OldResourceKey)))
        {
            if (flexible && originalKeys.Contains(d.Key))
            {
                continue;
            }

            // Queue rename
            var filter = Builders<ResourceRecord>.Filter.Eq(r => r.ResourceKey, d.OldResourceKey);
            var update = Builders<ResourceRecord>.Update
                .Set(r => r.ResourceKey, d.Key)
                .Set(r => r.FromCode, true);
            ops.Add(new UpdateOneModel<ResourceRecord>(filter, update));

            // Apply in-memory
            var item = snapshot.FirstOrDefault(r => r.ResourceKey == d.OldResourceKey);
            if (item != null)
            {
                item.ResourceKey = d.Key;
                item.FromCode = true;
            }
        }

        return ops;
    }

    private List<WriteModel<ResourceRecord>> BuildSyncOps(
        List<LocalizationResource> snapshot,
        IEnumerable<DiscoveredResource> discoveredResources)
    {
        var writeModels = new List<WriteModel<ResourceRecord>>();

        foreach (var discovered in discoveredResources)
        {
            var snapshotResource = snapshot.FirstOrDefault(r => r.ResourceKey == discovered.Key);

            if (snapshotResource == null)
            {
                // Insert new resource
                var newRecord = new ResourceRecord
                {
                    Id = NewResourceId(),
                    ResourceKey = discovered.Key,
                    ModificationDate = DateTime.UtcNow,
                    Author = "type-scanner",
                    FromCode = true,
                    IsModified = false,
                    IsHidden = discovered.IsHidden,
                    Translations = discovered.Translations
                        .Select(dt => new TranslationRecord
                        {
                            Id = NewTranslationId(),
                            Language = dt.Culture,
                            Value = dt.Translation,
                            ModificationDate = DateTime.UtcNow
                        })
                        .ToList(),
                    Notes = string.Empty
                };

                // Mirror into snapshot for further iterations
                var memoryResource = CreateResourceFromRecord(newRecord);
                snapshot.Add(memoryResource);

                writeModels.Add(new InsertOneModel<ResourceRecord>(newRecord));
            }
            else
            {
                // Base update: mark from code, hidden flag, and modification date
                var baseFilter = Builders<ResourceRecord>.Filter.Eq(r => r.ResourceKey, discovered.Key);
                var baseUpdate = Builders<ResourceRecord>.Update
                    .Set(r => r.FromCode, true)
                    .Set(r => r.IsHidden, discovered.IsHidden)
                    .Set(r => r.ModificationDate, DateTime.UtcNow);

                writeModels.Add(new UpdateOneModel<ResourceRecord>(baseFilter, baseUpdate));

                // Only auto‐sync translations if resource wasn't manually edited
                if (snapshotResource.IsModified == false)
                {
                    foreach (var discoveredTranslation in discovered.Translations)
                    {
                        var existingTranslation = snapshotResource.Translations
                            .SingleOrDefault(t => t.Language == discoveredTranslation.Culture);

                        if (existingTranslation == null)
                        {
                            // Push new translation
                            var translationToAdd = new TranslationRecord
                            {
                                Id = NewTranslationId(),
                                Language = discoveredTranslation.Culture,
                                Value = discoveredTranslation.Translation,
                                ModificationDate = DateTime.UtcNow
                            };

                            var pushUpdate = Builders<ResourceRecord>.Update
                                .Push(r => r.Translations, translationToAdd)
                                .Set(r => r.ModificationDate, DateTime.UtcNow);

                            writeModels.Add(new UpdateOneModel<ResourceRecord>(baseFilter, pushUpdate));

                            // Update in-memory snapshot
                            var translationRecord = CreateTranslationFromRecord(translationToAdd, snapshotResource);
                            snapshotResource.Translations.Add(translationRecord);
                        }
                        else if (existingTranslation.Value != discoveredTranslation.Translation)
                        {
                            // Build a filter that matches both the resource and the specific translation by Id
                            var positionalFilter = Builders<ResourceRecord>.Filter.And(
                                Builders<ResourceRecord>.Filter.Eq(r => r.ResourceKey, snapshotResource.ResourceKey),
                                Builders<ResourceRecord>.Filter.ElemMatch(
                                    r => r.Translations,
                                    tr => tr.Id == existingTranslation.Id
                                )
                            );

                            // Build an update to update the matched array element
                            var positionalUpdate = Builders<ResourceRecord>.Update
                                .Set(r => r.Translations.FirstMatchingElement().Value,
                                     discoveredTranslation.Translation)
                                .Set(r => r.Translations.FirstMatchingElement().ModificationDate, DateTime.UtcNow)
                                .Set(r => r.ModificationDate, DateTime.UtcNow);

                            writeModels.Add(new UpdateOneModel<ResourceRecord>(positionalFilter, positionalUpdate));

                            // Mirror in-memory snapshot
                            existingTranslation.Value = discoveredTranslation.Translation;
                        }
                    }
                }
            }
        }

        return writeModels;
    }

    private void ExecuteBulkWrite(IEnumerable<WriteModel<ResourceRecord>> operations)
    {
        // Materialize and short-circuit if there’s nothing to do
        var opList = operations.ToList();
        if (opList.Count == 0)
        {
            return;
        }

        // Send all updates/inserts in one go
        _collection.BulkWrite(opList);
    }


    private int NewResourceId()
    {
        return counterRepository.GetNextCounterValue(nameof(ResourceRecord));
    }

    private int NewTranslationId()
    {
        return counterRepository.GetNextCounterValue(nameof(TranslationRecord));
    }

    private LocalizationResource CreateResourceFromRecord(ResourceRecord resourceRecord)
    {
        var resource = new LocalizationResource(resourceRecord.ResourceKey, _enableInvariantCultureFallback)
        {
            Id = resourceRecord.Id,
            Author = resourceRecord.Author,
            FromCode = resourceRecord.FromCode,
            IsHidden = resourceRecord.IsHidden,
            IsModified = resourceRecord.IsModified,
            ModificationDate = resourceRecord.ModificationDate,
            Notes = resourceRecord.Notes
        };

        foreach (var translationRecord in resourceRecord.Translations)
        {
            var translation = CreateTranslationFromRecord(translationRecord, resource);
            resource.Translations.Add(translation);
        }

        return resource;
    }

    private LocalizationResourceTranslation CreateTranslationFromRecord(
        TranslationRecord translationRecord,
        LocalizationResource resource)
    {
        return new LocalizationResourceTranslation
        {
            Id = translationRecord.Id,
            ResourceId = resource.Id,
            Value = translationRecord.Value,
            Language = translationRecord.Language,
            ModificationDate = translationRecord.ModificationDate,
            LocalizationResource = resource
        };
    }
}
