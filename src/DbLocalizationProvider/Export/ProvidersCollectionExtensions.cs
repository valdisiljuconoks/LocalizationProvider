// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Export
{
    /// <summary>
    /// None reads this summary anyways..
    /// </summary>
    public static class ProvidersCollectionExtensions
    {
        /// <summary>
        /// Finds export implementation the by identifier (<see cref="IResourceExporter.ProviderId"/>).
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Resource exporter if found my <paramref name="id"/></returns>
        /// <exception cref="ArgumentNullException">id</exception>
        public static IResourceExporter FindById(this ICollection<IResourceExporter> list, string id)
        {
            if (string.IsNullOrEmpty(id))  throw new ArgumentNullException(nameof(id));

            return list.Single(p => p.ProviderId == id);
        }
    }
}
