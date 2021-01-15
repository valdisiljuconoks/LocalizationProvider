// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Removes single resource
    /// </summary>
    public class DeleteResourceHandler : ICommandHandler<DeleteResource.Command>
    {
        private readonly ConfigurationContext _configurationContext;
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        /// <param name="repository">Resource repository</param>
        public DeleteResourceHandler(ConfigurationContext configurationContext, IResourceRepository repository)
        {
            _configurationContext = configurationContext;
            _repository = repository;
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

            var resource = _repository.GetByKey(command.Key);

            if (resource == null)
            {
                return;
            }

            if (resource.FromCode)
            {
                throw new InvalidOperationException($"Cannot delete resource `{command.Key}` that is synced with code");
            }

            _repository.DeleteResource(resource);

            _configurationContext.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
