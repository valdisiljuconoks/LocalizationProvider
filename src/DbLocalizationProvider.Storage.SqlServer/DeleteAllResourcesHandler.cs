// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class DeleteAllResourcesHandler : ICommandHandler<DeleteAllResources.Command>
    {
        public void Execute(DeleteAllResources.Command command)
        {
            var repo = new ResourceRepository();
            repo.DeleteAllResources();
        }
    }
}
