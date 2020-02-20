// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Export
{
    /// <summary>
    /// Settings for the export operations
    /// </summary>
    public class ExportSettings
    {
        /// <summary>
        /// Gets the list of export providers.
        /// </summary>
        public ICollection<IResourceExporter> Providers { get; } = new List<IResourceExporter> { new JsonResourceExporter() };
    }
}
