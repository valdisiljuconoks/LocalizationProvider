// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Abstractions
{
    public interface IResourceRepository
    {
        IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant);

        LocalizationResource GetByKey(string resourceKey);

        void InsertResource(LocalizationResource resource);

        void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation);

        void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation);

        void UpdateResource(LocalizationResource resource);

        void DeleteAllResources();

        void DeleteResource(LocalizationResource resource);
    }
}
