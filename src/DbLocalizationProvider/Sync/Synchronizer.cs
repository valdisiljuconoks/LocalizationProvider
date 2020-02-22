// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// This class is responsible for trigger underlying storage schema sync process at correct time.
    /// </summary>
    public class Synchronizer
    {
        private static readonly ThreadSafeSingleShotFlag _synced = false;

        /// <summary>
        /// Updates the underlying storage schema.
        /// </summary>
        public void UpdateStorageSchema()
        {
            if (!_synced)
            {
                new UpdateSchema.Command().Execute();
            }
        }

        /// <summary>
        /// Synchronizes resources.
        /// </summary>
        public void SyncResources()
        {
            UpdateStorageSchema();

            var discoveredTypes = TypeDiscoveryHelper.GetTypes(
                t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            var discoveredResourceTypes = discoveredTypes[0];
            var discoveredModelTypes = discoveredTypes[1];
            var foreignResourceTypes = ConfigurationContext.Current.ForeignResources;

            if (foreignResourceTypes != null && foreignResourceTypes.Any())
            {
                discoveredResourceTypes.AddRange(foreignResourceTypes.Select(x => x.ResourceType));
            }

            ICollection<DiscoveredResource> discoveredResources = new List<DiscoveredResource>();
            ICollection<DiscoveredResource> discoveredModels = new List<DiscoveredResource>();

            Parallel.Invoke(() => discoveredResources = DiscoverResources(discoveredResourceTypes), () => discoveredModels = DiscoverResources(discoveredModelTypes));

            var syncCommand = new SyncResources.Query(discoveredResources, discoveredModels);
            var syncedResources = syncCommand.Execute();

            StoreKnownResourcesAndPopulateCache(syncedResources);
        }

        private ICollection<DiscoveredResource> DiscoverResources(List<Type> types)
        {
            var helper = new TypeDiscoveryHelper();
            var properties = types.SelectMany(type => helper.ScanResources(type)).DistinctBy(r => r.Key).ToList();

            return properties;
        }

        private static void StoreKnownResourcesAndPopulateCache(IEnumerable<LocalizationResource> syncedResources)
        {
            if (ConfigurationContext.Current.PopulateCacheOnStartup)
            {
                new ClearCache.Command().Execute();

                foreach (var resource in syncedResources)
                {
                    var key = CacheKeyHelper.BuildKey(resource.ResourceKey);
                    ConfigurationContext.Current.CacheManager.Insert(key, resource, true);
                }
            }
            else
            {
                // just store resource cache keys
                syncedResources.ForEach(r => ConfigurationContext.Current.BaseCacheManager.StoreKnownKey(r.ResourceKey));
            }
        }
    }
}
