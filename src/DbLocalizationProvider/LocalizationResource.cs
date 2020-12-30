// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Represents localizable resource
    /// </summary>
    [DebuggerDisplay("Key: {" + nameof(ResourceKey) + "}")]
    public class LocalizationResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationResource" /> class.
        /// </summary>
        public LocalizationResource() : this(null, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationResource" /> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="enableInvariantCultureFallback">Should we use invariant fallback or not.</param>
        public LocalizationResource(string key, bool enableInvariantCultureFallback)
        {
            ResourceKey = key;
            Translations = new LocalizationResourceTranslationCollection(enableInvariantCultureFallback);
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether resource is synced from code.
        /// </summary>
        public bool FromCode { get; set; }

        /// <summary>
        /// Gets or sets whether resource is modified from AdminUI.
        /// </summary>
        public bool? IsModified { get; set; }

        /// <summary>
        /// Gets or sets the is hidden.
        /// </summary>
        public bool? IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the notes for the resource.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets list of translations for the resource.
        /// </summary>
        public LocalizationResourceTranslationCollection Translations { get; internal set; }

        /// <summary>
        /// Creates new instance of resource that does not exist. This is required in cases when we need to cache non-existing resources.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Resource instance</returns>
        public static LocalizationResource CreateNonExisting(string key)
        {
            return new LocalizationResource(key, false) { Translations = null };
        }
    }
}
