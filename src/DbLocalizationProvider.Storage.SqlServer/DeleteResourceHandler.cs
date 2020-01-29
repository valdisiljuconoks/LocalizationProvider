// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class DeleteResourceHandler : ICommandHandler<DeleteResource.Command>
    {
        public void Execute(DeleteResource.Command command)
        {
            if (string.IsNullOrEmpty(command.Key)) throw new ArgumentNullException(nameof(command.Key));

            var repo = new ResourceRepository();
            var resource = repo.GetByKey(command.Key);

            if (resource == null) return;
            if (resource.FromCode)
            {
                throw new InvalidOperationException($"Cannot delete resource `{command.Key}` that is synced with code");
            }

            repo.DeleteResource(resource);

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
