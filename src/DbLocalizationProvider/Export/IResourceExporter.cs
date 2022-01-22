// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Collections.Specialized;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Export
{
    /// <summary>
    /// Interface for the export implementation
    /// </summary>
    public interface IResourceExporter
    {
        /// <summary>
        /// Gets the name of the export format (this will be visible on menu).
        /// </summary>
        string FormatName { get; }

        /// <summary>
        /// Gets the export provider identifier.
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Exports the specified resources.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Result of the export</returns>
        ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters);
    }
}
