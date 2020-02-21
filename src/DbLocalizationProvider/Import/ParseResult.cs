// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Import
{
    /// <summary>
    /// Result of the parsing operation
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <param name="detectedLanguages">The detected languages.</param>
        /// <exception cref="ArgumentNullException">
        /// resources
        /// or
        /// detectedLanguages
        /// </exception>
        public ParseResult(ICollection<LocalizationResource> resources, ICollection<CultureInfo> detectedLanguages)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            DetectedLanguages = detectedLanguages ?? throw new ArgumentNullException(nameof(detectedLanguages));
        }

        /// <summary>
        /// Gets the parsed resources.
        /// </summary>
        public ICollection<LocalizationResource> Resources { get; }

        /// <summary>
        /// Gets list of detected languages after parsing.
        /// </summary>
        public ICollection<CultureInfo> DetectedLanguages { get; }
    }
}
