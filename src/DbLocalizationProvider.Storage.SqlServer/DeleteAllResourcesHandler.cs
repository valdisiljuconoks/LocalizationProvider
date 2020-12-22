// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Astalavista all resources
    /// </summary>
    /// <seealso
    public class DeleteAllResourcesHandler : ICommandHandler<DeleteAllResources.Command>
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(DeleteAllResources.Command command)
        {
            var repo = new ResourceRepository();
            repo.DeleteAllResources();
        }
    }
}
