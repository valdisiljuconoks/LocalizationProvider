// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Command definition for deleting all resources
    /// </summary>
    public class DeleteAllResources
    {
        /// <summary>
        /// When you need to delete all resources (it might sounds crazy, but sometimes this is necessary) - execute this
        /// command.
        /// </summary>
        /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
        public class Command : ICommand { }
    }
}
