// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Astalavista all resources
    /// </summary>
    public class DeleteAllResourcesHandler : ICommandHandler<DeleteAllResources.Command>
    {
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="repository">Resource repository</param>
        public DeleteAllResourcesHandler(IResourceRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(DeleteAllResources.Command command)
        {
            _repository.DeleteAllResources();
        }
    }
}
