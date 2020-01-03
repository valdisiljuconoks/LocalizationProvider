// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Export
{
    public static class ProvidersCollectionExtensions
    {
        public static IResourceExporter FindById(this ICollection<IResourceExporter> list, string id)
        {
            if (string.IsNullOrEmpty(id))  throw new ArgumentNullException(nameof(id));

            return list.Single(p => p.ProviderId == id);
        }
    }
}
