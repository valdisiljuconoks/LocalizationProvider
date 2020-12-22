// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Deletes single resource.
    /// </summary>
    public class DeleteResource
    {
        /// <summary>
        /// Execute this command if you need to just delete single resource.
        /// </summary>
        /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
        public class Command : ICommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Command" /> class.
            /// </summary>
            /// <param name="key">The key.</param>
            public Command(string key)
            {
                Key = key;
            }

            /// <summary>
            /// Gets the key for the resource to delete.
            /// </summary>
            public string Key { get; }
        }
    }
}
