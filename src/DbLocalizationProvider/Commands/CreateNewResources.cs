// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    ///     This command is usually used when creating new resources either from AdminUI or during import process
    /// </summary>
    public class CreateNewResources
    {
        /// <summary>
        ///     This command is usually used when creating new resources either from AdminUI or during import process
        /// </summary>
        public class Command : ICommand
        {
            /// <summary>
            ///     Constructs new instance of command obviously.
            /// </summary>
            /// <param name="resources">List of resources to create</param>
            public Command(List<LocalizationResource> resources)
            {
                if (resources == null) throw new ArgumentNullException(nameof(resources));
                if (resources.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(resources));

                LocalizationResources = resources;
            }

            /// <summary>
            ///     List of resources to create. Resource instance should be fully filled in order to just commit to underlying
            ///     storage.
            /// </summary>
            public List<LocalizationResource> LocalizationResources { get; }
        }
    }
}
