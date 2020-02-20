// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    /// <summary>
    ///     Settings for import
    /// </summary>
    public class ImportSettings
    {
        /// <summary>
        ///     Gets the list of import providers.
        /// </summary>
        public ICollection<IResourceFormatParser> Providers { get; } =
            new List<IResourceFormatParser> { new JsonResourceFormatParser() };
    }
}
