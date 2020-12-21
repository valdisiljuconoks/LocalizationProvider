// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// You can use this interface to provide list of manually crafted resources those should be registered.
    /// </summary>
    public interface IManualResourceProvider
    {
        /// <summary>
        /// Return list of manually crafted resources.
        /// </summary>
        /// <returns>Return list of manually crafted resources.</returns>
        IEnumerable<ManualResource> GetResources();
    }
}
