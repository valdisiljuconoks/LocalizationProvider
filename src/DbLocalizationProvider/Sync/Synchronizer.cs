// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Logging;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// This class is responsible for trigger underlying storage schema sync process at correct time.
    /// </summary>
    public class Synchronizer : ISynchronizer
    {
        private static readonly ThreadSafeSingleShotFlag _synced = false;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IResourceRepository _repository;
        private readonly ILogger _logger;
        private readonly ConfigurationContext _configurationContext;
        private readonly TypeDiscoveryHelper _helper;
        private readonly IQueryExecutor _queryExecutor;

        /// <summary>
        /// Initializes new instance of the resource scanner.
        /// </summary>
        /// <param name="helper">Discovery helper to use to locate resources.</param>
        /// <param name="queryExecutor">The executor fo queries.</param>
        /// <param name="commandExecutor">The executor of commands.</param>
        /// <param name="repository">Resource repository.</param>
        /// <param name="logger">This guy will help us out in debug support cases.</param>
        /// <param name="configurationContext">Context of what has been configured.</param>
        public Synchronizer(
            TypeDiscoveryHelper helper,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IResourceRepository repository,
            ILogger logger,
            ConfigurationContext configurationContext)
        {
            _helper = helper;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _repository = repository;
            _logger = logger;
            _configurationContext = configurationContext;
        }

        /// <summary>
        /// Registers manually crafted resources.
        /// </summary>
        /// <param name="resources">List of resources.</param>
        public void RegisterManually(IEnumerable<ManualResource> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            foreach (var manualResource in resources)
            {
                var existingResource = _queryExecutor.Execute(new GetResource.Query(manualResource.Key));
                if (existingResource == null)
                {
                    var resourceToSync =
                        new LocalizationResource(manualResource.Key, _configurationContext.EnableInvariantCultureFallback)
                        {
                            Author = "manual",
                            FromCode = false,
                            IsModified = false,
                            IsHidden = false,
                            ModificationDate = DateTime.UtcNow
                        };

                    resourceToSync.Translations.Add(new LocalizationResourceTranslation
                    {
                        Language = manualResource.Language.Name,
                        Value = manualResource.Translation
                    });

                    _commandExecutor.Execute(new CreateNewResources.Command(new List<LocalizationResource> { resourceToSync }));
                }
                else
                {
                    _commandExecutor.Execute(
                        new CreateOrUpdateTranslation.Command(manualResource.Key,
                                                              manualResource.Language,
                                                              manualResource.Translation));
                }
            }
        }

        /// <summary>
        /// Updates the underlying storage schema.
        /// </summary>
        public void UpdateStorageSchema()
        {
            var command = new UpdateSchema.Command();
            if (!_commandExecutor.CanBeExecuted(command))
            {
                throw new InvalidOperationException(
                    "Resource sync handler is not registered. Make sure that storage provider is registered e.g. ctx.UseSqlServer(..)");
            }

            if (!_synced)
            {
                _commandExecutor.Execute(command);
            }
        }

        /// <summary>
        /// Synchronizes resources.
        /// </summary>
        /// <param name="registerResources">If <c>true</c> discovered resources are stored in underlying database</param>
        public void SyncResources(bool registerResources)
        {
            var resources = registerResources ? DiscoverReadMerge() : ReadMerge();

            StoreKnownResourcesAndPopulateCache(resources);
        }

        private IEnumerable<LocalizationResource> ReadMerge()
        {
            return _queryExecutor.Execute(new GetAllResources.Query(true));
        }

        private IEnumerable<LocalizationResource> DiscoverReadMerge()
        {
            UpdateStorageSchema();

            var discoveredTypes = _helper.GetTypes(
                t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            var discoveredResourceTypes = discoveredTypes[0];
            var discoveredModelTypes = discoveredTypes[1];
            var foreignResourceTypes = _configurationContext.ForeignResources;

            if (foreignResourceTypes != null && foreignResourceTypes.Any())
            {
                discoveredResourceTypes.AddRange(foreignResourceTypes.Select(x => x.ResourceType));
            }

            ICollection<DiscoveredResource> discoveredResources = new List<DiscoveredResource>();
            ICollection<DiscoveredResource> discoveredModels = new List<DiscoveredResource>();

            Parallel.Invoke(() => discoveredResources = DiscoverResources(discoveredResourceTypes),
                            () => discoveredModels = DiscoverResources(discoveredModelTypes));

            var syncedResources = Execute(discoveredResources, discoveredModels);

            return syncedResources;
        }

        private IEnumerable<LocalizationResource> Execute(
            ICollection<DiscoveredResource> discoveredResources,
            ICollection<DiscoveredResource> discoveredModels)
        {
            _logger.Debug("Starting to synchronize resources...");
            var sw = new Stopwatch();
            sw.Start();

            _repository.ResetSyncStatus();

            var allResources = _queryExecutor.Execute(new GetAllResources.Query(true));
            Parallel.Invoke(() => _repository.RegisterDiscoveredResources(discoveredResources, allResources),
                            () => _repository.RegisterDiscoveredResources(discoveredModels, allResources));

            var result = MergeLists(allResources, discoveredResources.ToList(), discoveredModels.ToList());
            sw.Stop();

            _logger.Debug($"Resource synchronization took: {sw.ElapsedMilliseconds}ms.");

            return result;
        }

        private ICollection<DiscoveredResource> DiscoverResources(List<Type> types)
        {
            var properties = types.SelectMany(type => _helper.ScanResources(type)).DistinctBy(r => r.Key).ToList();

            return properties;
        }

        private void StoreKnownResourcesAndPopulateCache(IEnumerable<LocalizationResource> syncedResources)
        {
            if (_configurationContext.PopulateCacheOnStartup)
            {
                _commandExecutor.Execute(new ClearCache.Command());

                foreach (var resource in syncedResources)
                {
                    var key = CacheKeyHelper.BuildKey(resource.ResourceKey);
                    _configurationContext.CacheManager.Insert(key, resource, true);
                }
            }
            else
            {
                // just store resource cache keys
                syncedResources.ForEach(r => _configurationContext.BaseCacheManager.StoreKnownKey(r.ResourceKey));
            }
        }

        internal IEnumerable<LocalizationResource> MergeLists(
            IEnumerable<LocalizationResource> databaseResources,
            List<DiscoveredResource> discoveredResources,
            List<DiscoveredResource> discoveredModels)
        {
            if (discoveredResources == null)
            {
                throw new ArgumentNullException(nameof(discoveredResources));
            }

            if (discoveredModels == null)
            {
                throw new ArgumentNullException(nameof(discoveredModels));
            }

            if (!discoveredResources.Any() && !discoveredModels.Any())
            {
                return databaseResources;
            }

            var result = new List<LocalizationResource>(databaseResources);
            var dic = result.ToDictionary(r => r.ResourceKey, r => r);

            // run through resources
            CompareAndMerge(ref discoveredResources, dic, ref result);
            CompareAndMerge(ref discoveredModels, dic, ref result);

            return result;
        }

        private void CompareAndMerge(
            ref List<DiscoveredResource> discoveredResources,
            Dictionary<string, LocalizationResource> dic,
            ref List<LocalizationResource> result)
        {
            while (discoveredResources.Count > 0)
            {
                var discoveredResource = discoveredResources[0];
                if (!dic.ContainsKey(discoveredResource.Key))
                {
                    // there is no resource by this key in db - we can safely insert
                    var resourceToAdd = new LocalizationResource(
                        discoveredResource.Key,
                        _configurationContext.EnableInvariantCultureFallback);

                    resourceToAdd.Translations.AddRange(
                        discoveredResource
                            .Translations
                            .Select(t => new LocalizationResourceTranslation { Language = t.Culture, Value = t.Translation })
                            .ToList());

                    result.Add(resourceToAdd);
                }
                else
                {
                    // resource exists in db - we need to merge only unmodified translations
                    var existingRes = dic[discoveredResource.Key];
                    if (!existingRes.IsModified.HasValue || !existingRes.IsModified.Value)
                    {
                        // resource is unmodified in db - overwrite
                        foreach (var translation in discoveredResource.Translations)
                        {
                            var t = existingRes.Translations.FindByLanguage(translation.Culture);
                            if (t == null)
                            {
                                existingRes.Translations.Add(new LocalizationResourceTranslation
                                {
                                    Language = translation.Culture,
                                    Value = translation.Translation
                                });
                            }
                            else
                            {
                                t.Language = translation.Culture;
                                t.Value = translation.Translation;
                            }
                        }
                    }
                    else
                    {
                        // resource exists in db, is modified - we need to update only invariant translation if we have one :)
                        var t = existingRes.Translations.FindByLanguage(CultureInfo.InvariantCulture);
                        if (t != null)
                        {
                            var invariant = discoveredResource.Translations.FirstOrDefault(t2 => t.Language == string.Empty);
                            if (invariant != null)
                            {
                                t.Language = invariant.Culture;
                                t.Value = invariant.Translation;
                            }
                        }
                    }
                }

                discoveredResources.Remove(discoveredResource);
            }
        }
    }
}
