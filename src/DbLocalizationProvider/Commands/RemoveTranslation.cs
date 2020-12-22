// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Class when you need to just translation in some language for given resource.
    /// </summary>
    public class RemoveTranslation
    {
        /// <summary>
        /// Execute this command if you need to just translation in some language for given resource.
        /// </summary>
        /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
        public class Command : ICommand
        {
            /// <summary>
            /// Execute this command if you need to just translation in some language for given resource.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="language">The language.</param>
            /// <exception cref="ArgumentNullException">
            /// If <paramref name="key" /> or <paramref name="language" /> is null.
            /// </exception>
            public Command(string key, CultureInfo language)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Language = language ?? throw new ArgumentNullException(nameof(language));
            }

            /// <summary>
            /// Gets the resource key.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Gets the language for translation to remove from resource.
            /// </summary>
            public CultureInfo Language { get; }
        }
    }
}
