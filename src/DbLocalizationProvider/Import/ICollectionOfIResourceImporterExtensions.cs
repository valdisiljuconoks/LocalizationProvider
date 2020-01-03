// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    public static class ICollectionOfIResourceImporterExtensions
    {
        public static IResourceFormatParser FindByExtension(this ICollection<IResourceFormatParser> providers, string extension)
        {
            var lowered = extension.ToLower();

            return providers.FirstOrDefault(p => p.SupportedFileExtensions.Contains(lowered));
        }
    }
}
