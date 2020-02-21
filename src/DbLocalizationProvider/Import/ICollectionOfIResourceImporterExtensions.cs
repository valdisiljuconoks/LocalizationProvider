// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    /// <summary>
    /// Who cares?
    /// </summary>
    public static class ICollectionOfIResourceImporterExtensions
    {
        /// <summary>
        /// Finds resource importer the by file extension.
        /// </summary>
        /// <param name="providers">The providers.</param>
        /// <param name="extension">The file extension.</param>
        /// <returns>Resource importer for given file extension (if one is registered)</returns>
        public static IResourceFormatParser FindByExtension(this ICollection<IResourceFormatParser> providers, string extension)
        {
            var lowered = extension.ToLower();

            return providers.FirstOrDefault(p => p.SupportedFileExtensions.Contains(lowered));
        }
    }
}
