// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Wanna create new resource manually during runtime? This is the command the execute.
    /// </summary>
    public class CreateNewResource
    {
        /// <summary>
        /// Implementation of the command to create new resources
        /// </summary>
        public class Handler : ICommandHandler<Command>
        {
            private readonly ICommandExecutor _commandExecutor;

            /// <summary>
            /// Creates new instance
            /// </summary>
            /// <param name="commandExecutor">This is required to forward call to another command handler.</param>
            public Handler(ICommandExecutor commandExecutor)
            {
                _commandExecutor = commandExecutor;
            }

            /// <summary>
            /// What do you think this method does?
            /// </summary>
            /// <param name="command">Create resource command.</param>
            public void Execute(Command command)
            {
                _commandExecutor.Execute(
                    new CreateNewResources.Command(new List<LocalizationResource> { command.LocalizationResource }));
            }
        }

        /// <summary>
        /// This command is usually used when creating new resources either from AdminUI or during import process
        /// </summary>
        public class Command : ICommand
        {
            /// <summary>
            /// Constructs new instance of command obviously.
            /// </summary>
            /// <param name="resource">Resource to create</param>
            public Command(LocalizationResource resource)
            {
                LocalizationResource = resource ?? throw new ArgumentNullException(nameof(resource));
            }

            /// <summary>
            /// List of resources to create. Resource instance should be fully filled in order to just commit to underlying
            /// storage.
            /// </summary>
            public LocalizationResource LocalizationResource { get; }
        }

        /// <summary>
        /// Arguments for the event handlers
        /// </summary>
        public class EventArgs : System.EventArgs
        {
            /// <summary>
            /// Creates new instance of argument class
            /// </summary>
            /// <param name="key">Resource key which has been created</param>
            public EventArgs(string key)
            {
                Key = key;
            }

            /// <summary>
            /// Resource key which has been created
            /// </summary>
            public string Key { get; }
        }
    }
}
