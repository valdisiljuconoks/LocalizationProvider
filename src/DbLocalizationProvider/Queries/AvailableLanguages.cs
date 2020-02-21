// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    ///     When you need to get al available/supported languages
    /// </summary>
    public class AvailableLanguages
    {
        /// <summary>
        ///     Query definition of the all available/supported languages
        /// </summary>
        public class Query : IQuery<IEnumerable<CultureInfo>>
        {
            /// <summary>
            ///     To control whether you would like to include all invariant translations as well
            /// </summary>
            public bool IncludeInvariant { get; set; }
        }
    }
}
