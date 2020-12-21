// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// Ensures that resources from code and/or manually crafted are pushed down to underlying storage.
    /// </summary>
    public interface ISynchronizer
    {
        /// <summary>
        /// Synchronizes manually crafted resources
        /// </summary>
        /// <param name="resources"></param>
        void RegisterManually(IEnumerable<ManualResource> resources);
    }
}
