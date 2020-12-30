// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    /// <summary>
    /// Removes single resource
    /// </summary>
    public class DeleteResourceHandler : ICommandHandler<DeleteResource.Command>
    {
        private readonly ConfigurationContext _configurationContext;

        /// <summary>
        /// Creates new instance of the handler.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        public DeleteResourceHandler(ConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext;
        }

        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        /// <exception cref="ArgumentNullException">Key</exception>
        /// <exception cref="InvalidOperationException">Cannot delete resource `{command.Key}` that is synced with code</exception>
        public void Execute(DeleteResource.Command command)
        {
            if (string.IsNullOrEmpty(command.Key))
            {
                throw new ArgumentNullException(nameof(command.Key));
            }

            var repo = new ResourceRepository(_configurationContext);
            var resource = repo.GetByKey(command.Key);

            if (resource == null)
            {
                return;
            }

            if (resource.FromCode)
            {
                throw new InvalidOperationException($"Cannot delete resource `{command.Key}` that is synced with code");
            }

            repo.DeleteResource(resource);

            _configurationContext.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
