// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Commands;

/// <summary>
/// Deletes single resource.
/// </summary>
public class DeleteResource
{
    /// <summary>
    /// Removes single resource
    /// </summary>
    public class Handler : ICommandHandler<Command>
    {
        private readonly IOptions<ConfigurationContext> _configurationContext;
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        /// <param name="repository">Resource repository</param>
        public Handler(IOptions<ConfigurationContext> configurationContext, IResourceRepository repository)
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
        public void Execute(Command command)
        {
            if (string.IsNullOrEmpty(command.Key))
            {
                throw new ArgumentException("command.Key is null or empty");
            }

            var resource = _repository.GetByKey(command.Key);

            if (resource == null)
            {
                return;
            }

            if (resource.FromCode && !command.IgnoreFromCode)
            {
                throw new InvalidOperationException($"Cannot delete resource `{command.Key}` that is synced with code");
            }

            _repository.DeleteResource(resource);

            _configurationContext.Value.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }

    /// <summary>
    /// Execute this command if you need to just delete single resource.
    /// </summary>
    /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
    public class Command : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public Command(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets the key for the resource to delete.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Set this flag to <c>true</c> to ignore <c>FromCode</c> check (if resource is synced from code - it cannot be deleted).
        /// </summary>
        public bool IgnoreFromCode { get; set; }
    }
}
