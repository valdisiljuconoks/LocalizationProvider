// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Storage.AzureTables
{
    /// <summary>
    /// Repository for working with underlying Azure Tables storage.
    /// </summary>
    public class ResourceRepository : IResourceRepository
    {
        private readonly bool _enableInvariantCultureFallback;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        public ResourceRepository(ConfigurationContext configurationContext)
        {
            _enableInvariantCultureFallback = configurationContext.EnableInvariantCultureFallback;
            _logger = configurationContext.Logger;
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns>List of resources</returns>
        public async Task<IEnumerable<LocalizationResource>> GetAllAsync()
        {
            try
            {
                var table = GetTableClient();
                var result = await table.ExecuteQueryAsync<LocalizationResourceEntity>(CreateResourcesByPartitionFilter());

                return result.Select(FromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger?.Error("Failed to retrieve all resources.", ex);
                return Enumerable.Empty<LocalizationResource>();
            }
        }

        private static Expression<Func<LocalizationResourceEntity, bool>> CreateResourcesByPartitionFilter()
        {
            return e => e.PartitionKey == LocalizationResourceEntity.PartitionKeyValue;
        }

        /// <summary>
        /// Gets resource by the key.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>Localized resource if found by given key</returns>
        /// <exception cref="ArgumentNullException">resourceKey</exception>
        public async Task<LocalizationResource> GetByKeyAsync(string resourceKey)
        {
            if (resourceKey == null)
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            try
            {
                var entity = await GetEntityByKeyAsync(resourceKey);
                return FromEntity(entity);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to retrieve resource by key {resourceKey}.", ex);
                return null;
            }
        }

        private static async Task<LocalizationResourceEntity> GetEntityByKeyAsync(string resourceKey)
        {
            var table = GetTableClient();
            return await table.GetEntityAsync<LocalizationResourceEntity>(LocalizationResourceEntity.PartitionKeyValue, resourceKey);
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
        public async Task AddTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            resource.Translations.Add(translation);

            var client = GetTableClient();
            var entity = new LocalizationResourceEntity(resource.ResourceKey);
            Map(resource, entity);
            await client.UpsertEntityAsync(entity);
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
        public async Task UpdateTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            await SaveAsync(resource);
        }

        private async Task SaveAsync(LocalizationResource resource)
        {
            var table = GetTableClient();
            var entity = await GetEntityByKeyAsync(resource.ResourceKey);
            Map(resource, entity);
            await table.UpsertEntityAsync(entity);
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
        public async Task DeleteTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            resource.Translations.Remove(resource.Translations.FindByLanguage(translation.Language));

            await SaveAsync(resource);
        }

        /// <summary>
        /// Updates the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public async Task UpdateResourceAsync(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            await SaveAsync(resource);
        }

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public async Task DeleteResourceAsync(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var table = GetTableClient();
            await DeleteEntityAsync(LocalizationResourceEntity.PartitionKeyValue, resource.ResourceKey, table);
        }

        private static async Task DeleteEntityAsync(string partitionKey, string rowKey, TableClient table)
        {
            await table.DeleteEntityAsync(partitionKey, rowKey);
        }

        /// <summary>
        /// Deletes all resources. DANGEROUS!
        /// </summary>
        public async Task DeleteAllResourcesAsync()
        {
            var table = GetTableClient();
            var allResources = await GetAllAsync();

            foreach (var key in allResources.Select(r => r.ResourceKey))
            {
                await DeleteEntityAsync(LocalizationResourceEntity.PartitionKeyValue, key, table);
            }
        }

        /// <summary>
        /// Inserts the resource in database.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public async Task InsertResourceAsync(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var table = GetTableClient();
            var entity = new LocalizationResourceEntity(resource.ResourceKey);
            Map(resource, entity);
            await table.AddEntityAsync(entity);
        }

        /// <summary>
        /// Gets the available languages (reads in which languages translations are added).
        /// </summary>
        /// <param name="includeInvariant">if set to <c>true</c> include invariant.</param>
        /// <returns>List of all available languages</returns>
        public async Task<IEnumerable<CultureInfo>> GetAvailableLanguagesAsync(bool includeInvariant)
        {
            try
            {
                var allResources = await GetAllAsync();

                return allResources
                    .SelectMany(r => r.Translations.Select(t => t.Language))
                    .Distinct()
                    .Where(l => includeInvariant || l != string.Empty)
                    .Select(l => new CultureInfo(l));
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to retrieve all available languages.", ex);
                return Enumerable.Empty<CultureInfo>();
            }
        }

        /// <summary>
        ///Resets synchronization status of the resources.
        /// </summary>
        public async Task ResetSyncStatusAsync()
        {
            var allResources = await GetAllAsync();
            var allKeys = allResources.Select(r => r.ResourceKey);
            var table = GetTableClient();

            foreach (var key in allKeys)
            {
                var entity = await GetEntityByKeyAsync(key);
                entity.FromCode = false;
                await table.UpsertEntityAsync(entity);
            }
        }

        /// <summary>
        /// Registers discovered resources.
        /// </summary>
        /// <param name="discoveredResources">Collection of discovered resources during scanning process.</param>
        /// <param name="allResources">All existing resources (so you could compare and decide what script to generate).</param>
        /// <param name="flexibleRefactoringMode"></param>
        public async Task RegisterDiscoveredResources(
            ICollection<DiscoveredResource> discoveredResources,
            IEnumerable<LocalizationResource> allResources,
            bool flexibleRefactoringMode)
        {
            foreach (var discoveredResource in discoveredResources)
            {
                var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == discoveredResource.Key);
                if (existingResource == null)
                {
                    await InsertResourceAsync(ToResource(discoveredResource));
                }

                if (existingResource != null)
                {
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

                    await SaveAsync(existingResource);
                }
            }
        }

        private static TableClient GetTableClient() => new(Settings.ConnectionString, "LocalizationResources");

        private LocalizationResource ToResource(DiscoveredResource discoveredResource)
        {
            var resource = new LocalizationResource(discoveredResource.Key, true)
            {
                Author = "type-scanner",
                ModificationDate = DateTime.UtcNow,
                FromCode = true,
                IsModified = false,
                IsHidden = discoveredResource.IsHidden,
            };

            resource.Translations.AddRange(discoveredResource.Translations.Select(ToTranslation));
            return resource;
        }

        private static LocalizationResourceTranslation ToTranslation(DiscoveredTranslation translation)
        {
            return new LocalizationResourceTranslation
            {
                Language = translation.Culture,
                Value = translation.Translation,
                ModificationDate = DateTime.UtcNow
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
                Language = translation.Language,
                Translation = translation.Value,
                ModificationDate = DateTime.UtcNow
            };
        }

        private LocalizationResource FromEntity(LocalizationResourceEntity firstOrDefault)
        {
            if (firstOrDefault == null) return null;

            var result = new LocalizationResource(firstOrDefault.RowKey, _enableInvariantCultureFallback)
            {
                Author = firstOrDefault.Author,
                ModificationDate = firstOrDefault.ModificationDate,
                FromCode = firstOrDefault.FromCode,
                IsModified = firstOrDefault.IsModified,
                IsHidden = firstOrDefault.IsHidden
            };

            var translationEntities = JsonConvert.DeserializeObject<LocalizationResourceTranslationEntity[]>(firstOrDefault.Translations);

            if (translationEntities.Any())
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
}
