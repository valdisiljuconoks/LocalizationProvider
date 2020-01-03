// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Collections.Specialized;

namespace DbLocalizationProvider.Export
{
    public interface IResourceExporter
    {
        string FormatName { get; }

        string ProviderId { get; }

        ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters);
    }
}
