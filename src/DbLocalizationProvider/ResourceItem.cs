// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;

namespace DbLocalizationProvider
{
    public class ResourceItem
    {
        public ResourceItem(string key, string value, CultureInfo sourceCulture)
        {
            Key = key;
            Value = value;
            SourceCulture = sourceCulture;
        }

        public string Key { get; }

        public string Value { get; }

        public CultureInfo SourceCulture { get; }
    }
}
