// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// Repository interface to be implemented by some storage provider.
    /// </summary>
    public interface IResourceRepository
    {
        Task<IEnumerable<CultureInfo>> GetAvailableLanguagesAsync(bool includeInvariant);

        Task<LocalizationResource> GetByKeyAsync(string resourceKey);

        Task InsertResourceAsync(LocalizationResource resource);

        Task AddTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation);

        Task UpdateTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation);

        Task UpdateResourceAsync(LocalizationResource resource);

        Task DeleteAllResourcesAsync();

        Task DeleteResourceAsync(LocalizationResource resource);

        Task<IEnumerable<LocalizationResource>> GetAllAsync();

        Task DeleteTranslationAsync(LocalizationResource resource, LocalizationResourceTranslation translation);

        Task ResetSyncStatusAsync();

        Task RegisterDiscoveredResources(
            ICollection<DiscoveredResource> discoveredResources,
            IEnumerable<LocalizationResource> allResources,
            bool flexibleRefactoringMode);
    }
}
