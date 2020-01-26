// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DbLocalizationProvider
{
    [DebuggerDisplay("Key: {" + nameof(ResourceKey) + "}")]
    public class LocalizationResource
    {
        public LocalizationResource() : this(null) { }

        public LocalizationResource(string key)
        {
            ResourceKey = key;
        }

        public int Id { get; set; }

        public string ResourceKey { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Author { get; set; }

        public bool FromCode { get; set; }

        public bool? IsModified { get; set; }

        public bool? IsHidden { get; set; }

        public string Notes { get; set; }

        public ICollection<LocalizationResourceTranslation> Translations { get; set; } = new List<LocalizationResourceTranslation>();

        public static LocalizationResource CreateNonExisting(string key)
        {
            return new LocalizationResource(key) { Translations = null };
        }
    }
}
