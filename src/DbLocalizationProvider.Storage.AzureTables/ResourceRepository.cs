// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Storage.AzureTables
{
    /// <summary>
    /// Repository for working with underlying Azure Tables storage.
    /// </summary>
    public class ResourceRepository : IResourceRepository
    {
        private readonly bool _enableInvariantCultureFallback;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        public ResourceRepository(ConfigurationContext configurationContext)
        {
            _enableInvariantCultureFallback = configurationContext.EnableInvariantCultureFallback;
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns>List of resources</returns>
        public IEnumerable<LocalizationResource> GetAll()
        {
            var partitionCondition = TableQuery.GenerateFilterCondition(nameof(LocalizationResourceEntity.PartitionKey),
                                                                        QueryComparisons.Equal,
                                                                        LocalizationResourceEntity.PartitionKey);

            var query = new TableQuery<LocalizationResourceEntity>().Where(partitionCondition);
            var table = GetTable();
            var result = table.ExecuteQuery(query);

            return result.Select(FromEntity).ToList();
        }

        /// <summary>
        /// Gets resource by the key.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>Localized resource if found by given key</returns>
        /// <exception cref="ArgumentNullException">resourceKey</exception>
        public LocalizationResource GetByKey(string resourceKey)
        {
            if (resourceKey == null)
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            var partitionCondition = TableQuery.GenerateFilterCondition(nameof(LocalizationResourceEntity.PartitionKey),
                                                                        QueryComparisons.Equal,
                                                                        LocalizationResourceEntity.PartitionKey);

            var keyCondition = TableQuery.GenerateFilterCondition("RowKey",
                                                                  QueryComparisons.Equal,
                                                                  resourceKey);

            var theCondition = TableQuery.CombineFilters(partitionCondition, TableOperators.And, keyCondition);
            var query = new TableQuery<LocalizationResourceEntity>().Where(theCondition);
            var table = GetTable();
            var result = table.ExecuteQuery(query);

            return FromEntity(result.FirstOrDefault());
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
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            resource.Translations.Add(translation);

            var table = GetTable();
            table.Execute(TableOperation.InsertOrReplace(ToEntity(resource)));
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
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            var table = GetTable();
            table.Execute(TableOperation.Replace(ToEntity(resource)));
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
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (translation == null)
            {
                throw new ArgumentNullException(nameof(translation));
            }

            resource.Translations.Remove(resource.Translations.FindByLanguage(translation.Language));

            var table = GetTable();
            table.Execute(TableOperation.Replace(ToEntity(resource)));
        }

        /// <summary>
        /// Updates the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void UpdateResource(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var table = GetTable();
            var entity = new DynamicTableEntity(LocalizationResourceEntity.PartitionKey, resource.ResourceKey) { ETag = "*" };

            entity.Properties.Add(nameof(LocalizationResourceEntity.FromCode), new EntityProperty(resource.FromCode));
            entity.Properties.Add(nameof(LocalizationResourceEntity.ModificationDate), new EntityProperty(resource.ModificationDate));
            entity.Properties.Add(nameof(LocalizationResourceEntity.IsModified), new EntityProperty(resource.IsModified));
            entity.Properties.Add(nameof(LocalizationResourceEntity.IsHidden), new EntityProperty(resource.IsHidden));
            entity.Properties.Add(nameof(LocalizationResourceEntity.Notes), new EntityProperty(resource.Notes));

            var mergeOp = TableOperation.Merge(entity);
            table.Execute(mergeOp);
        }

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void DeleteResource(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var table = GetTable();
            var entity = new DynamicTableEntity(LocalizationResourceEntity.PartitionKey, resource.ResourceKey) { ETag = "*" };

            table.Execute(TableOperation.Delete(entity));
        }

        /// <summary>
        /// Deletes all resources. DANGEROUS!
        /// </summary>
        public void DeleteAllResources()
        {
            var table = GetTable();
            foreach (var key in GetAll().Select(r => r.ResourceKey))
            {
                var entity = new DynamicTableEntity(LocalizationResourceEntity.PartitionKey, key) { ETag = "*" };
                table.Execute(TableOperation.Delete(entity));
            }
        }

        /// <summary>
        /// Inserts the resource in database.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <exception cref="ArgumentNullException">resource</exception>
        public void InsertResource(LocalizationResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var table = GetTable();
            table.Execute(TableOperation.Insert(ToEntity(resource)));
        }

        /// <summary>
        /// Gets the available languages (reads in which languages translations are added).
        /// </summary>
        /// <param name="includeInvariant">if set to <c>true</c> include invariant.</param>
        /// <returns>List of all available languages</returns>
        public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant)
        {
            var allResources = GetAll();

            return allResources
                .SelectMany(r => r.Translations.Select(t => t.Language))
                .Distinct()
                .Where(l => includeInvariant || l != string.Empty)
                .Select(l => new CultureInfo(l));
        }

        /// <summary>
        ///Resets synchronization status of the resources.
        /// </summary>
        public void ResetSyncStatus()
        {
            var allKeys = GetAll().Select(r => r.ResourceKey);
            var table = GetTable();

            foreach (var key in allKeys)
            {
                var entity = new DynamicTableEntity(LocalizationResourceEntity.PartitionKey, key) { ETag = "*" };
                entity.Properties.Add(nameof(LocalizationResourceEntity.FromCode), new EntityProperty(false));
                var mergeOp = TableOperation.Merge(entity);
                table.Execute(mergeOp);
            }
        }

        /// <summary>
        /// Registers discovered resources.
        /// </summary>
        /// <param name="discoveredResources">Collection of discovered resources during scanning process.</param>
        /// <param name="allResources">All existing resources (so you could compare and decide what script to generate).</param>
        /// <param name="flexibleRefactoringMode"></param>
        public void RegisterDiscoveredResources(
            ICollection<DiscoveredResource> discoveredResources,
            IEnumerable<LocalizationResource> allResources,
            bool flexibleRefactoringMode)
        {
            var table = GetTable();

            foreach (var discoveredResource in discoveredResources)
            {
                var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == discoveredResource.Key);
                if (existingResource == null)
                {
                    table.Execute(TableOperation.InsertOrReplace(ToEntity(discoveredResource)));
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

                    table.Execute(TableOperation.InsertOrReplace(ToEntity(existingResource)));
                }
            }
        }

        private static CloudTable GetTable()
        {
            var storageAccount = CloudStorageAccount.Parse(Settings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference("LocalizationResources");

            return table;
        }

        private LocalizationResourceEntity ToEntity(DiscoveredResource discoveredResource)
        {
            return new LocalizationResourceEntity(discoveredResource.Key)
            {
                Author = "type-scanner",
                ModificationDate = DateTime.UtcNow,
                FromCode = true,
                IsModified = false,
                IsHidden = discoveredResource.IsHidden,
                Translations = JsonConvert.SerializeObject(discoveredResource.Translations.Select(ToTranslationEntity).ToList())
            };
        }

        private LocalizationResourceTranslationEntity ToTranslationEntity(DiscoveredTranslation translation)
        {
            return new LocalizationResourceTranslationEntity
            {
                Language = translation.Culture, Translation = translation.Translation, ModificationDate = DateTime.UtcNow
            };
        }

        private ITableEntity ToEntity(LocalizationResource resource)
        {
            return new LocalizationResourceEntity(resource.ResourceKey)
            {
                Author = resource.Author,
                ModificationDate = DateTime.UtcNow,
                FromCode = resource.FromCode,
                IsModified = resource.IsModified ?? true,
                IsHidden = resource.IsHidden ?? false,
                Translations = JsonConvert.SerializeObject(resource.Translations.Select(ToTranslationEntity).ToList()),
                ETag = "*"
            };
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
