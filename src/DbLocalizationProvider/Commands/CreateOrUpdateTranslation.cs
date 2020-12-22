// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Create or update translation for existing resource in given language.
    /// </summary>
    public class CreateOrUpdateTranslation
    {
        /// <summary>
        /// Command definition for creating or updating translation for existing resource in given language.
        /// </summary>
        /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
        public class Command : ICommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Command" /> class.
            /// </summary>
            /// <param name="key">The resource key.</param>
            /// <param name="language">The language for the translation.</param>
            /// <param name="translation">The actual translation for given language.</param>
            public Command(string key, CultureInfo language, string translation)
            {
                Key = key;
                Language = language;
                Translation = translation;
            }

            /// <summary>
            /// Gets the resource key.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Gets the language.
            /// </summary>
            public CultureInfo Language { get; }

            /// <summary>
            /// Gets the translation for given <see cref="Language" />.
            /// </summary>
            public string Translation { get; }
        }
    }
}
