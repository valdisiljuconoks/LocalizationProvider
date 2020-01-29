// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class CreateNewResourcesHandler : ICommandHandler<CreateNewResources.Command>
    {
        public void Execute(CreateNewResources.Command command)
        {
            if (command.LocalizationResources == null || !command.LocalizationResources.Any()) return;

            var repo = new ResourceRepository();

            foreach (var resource in command.LocalizationResources)
            {
                var existingResource = repo.GetByKey(resource.ResourceKey);

                if (existingResource != null)
                {
                    throw new InvalidOperationException($"Resource with key `{resource.ResourceKey}` already exists");
                }

                resource.ModificationDate = DateTime.UtcNow;
                repo.InsertResource(resource);
            }
        }
    }
}
