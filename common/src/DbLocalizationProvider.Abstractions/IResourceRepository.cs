// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Repository interface to be implemented by some storage provider.
/// </summary>
public interface IResourceRepository
{
    /// <summary>
    /// Gets the available languages (reads in which languages translations are added).
    /// </summary>
    /// <param name="includeInvariant">if set to <c>true</c> include invariant.</param>
    /// <returns>List of all available languages</returns>
    IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant);

    /// <summary>
    /// Gets resource by the key.
    /// </summary>
    /// <param name="resourceKey">The resource key.</param>
    /// <returns>Localized resource if found by given key</returns>
    /// <exception cref="ArgumentNullException">resourceKey</exception>
    LocalizationResource? GetByKey(string resourceKey);

    /// <summary>
    /// Inserts the resource in database.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    void InsertResource(LocalizationResource resource);

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
    void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation);

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
    void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation);

    /// <summary>
    /// Updates the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    void UpdateResource(LocalizationResource resource);

    /// <summary>
    /// Deletes all resources. DANGEROUS!
    /// </summary>
    void DeleteAllResources();

    /// <summary>
    /// Deletes the resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <exception cref="ArgumentNullException">resource</exception>
    void DeleteResource(LocalizationResource resource);

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <returns>List of resources</returns>
    IEnumerable<LocalizationResource> GetAll();

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
    void DeleteTranslation(LocalizationResource resource, LocalizationResourceTranslation translation);

    /// <summary>
    /// Resets synchronization status of the resources.
    /// </summary>
    void ResetSyncStatus();

    /// <summary>
    /// Registers discovered resources.
    /// </summary>
    /// <param name="discoveredResources">Collection of discovered resources during scanning process.</param>
    /// <param name="allResources">All existing resources (so you could compare and decide what script to generate).</param>
    /// <param name="flexibleRefactoringMode">I'm not quite sure anymore how this was used and why it was needed.</param>
    /// <param name="source">Source of the sync.</param>
    void RegisterDiscoveredResources(
        ICollection<DiscoveredResource> discoveredResources,
        Dictionary<string, LocalizationResource> allResources,
        bool flexibleRefactoringMode,
        SyncSource source);
}
