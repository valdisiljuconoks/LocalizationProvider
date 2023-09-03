// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Command definition for deleting all resources
    /// </summary>
    public class DeleteAllResources
    {
        /// <summary>
        /// Astalavista all resources
        /// </summary>
        public class Handler : ICommandHandler<Command>
        {
            private readonly IResourceRepository _repository;

            /// <summary>
            /// Creates new instance of the class.
            /// </summary>
            /// <param name="repository">Resource repository</param>
            public Handler(IResourceRepository repository)
            {
                _repository = repository;
            }

            /// <summary>
            /// Handles the command. Actual instance of the command being executed is passed-in as argument
            /// </summary>
            /// <param name="command">Actual command instance being executed</param>
            public async Task Execute(Command command)
            {
                await _repository.DeleteAllResourcesAsync();
            }
        }

        /// <summary>
        /// When you need to delete all resources (it might sounds crazy, but sometimes this is necessary) - execute this
        /// command.
        /// </summary>
        /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
        public class Command : ICommand { }
    }
}
