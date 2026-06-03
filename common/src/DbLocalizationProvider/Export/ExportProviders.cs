// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Export;

/// <summary>
/// Collector of various exporters
/// </summary>
public class ExportProviders
{
    /// <summary>
    /// Gets list of export providers.
    /// </summary>
    public ICollection<IResourceExporter> Providers { get; } = [new JsonResourceExporter()];
}
