// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Commands;

/// <summary>
/// Deletes multiple resources in a single operation.
/// </summary>
public class BulkDeleteResources
{
    /// <summary>
    /// Removes a batch of resources.
    /// </summary>
    public class Handler(IOptions<ConfigurationContext> configurationContext, IResourceRepository repository)
        : ICommandHandler<Command>
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument.
        /// Resources synced from code are silently skipped unless <see cref="Command.IgnoreFromCode"/> is set.
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(Command command)
        {
            ArgumentNullException.ThrowIfNull(command);

            if (command.Keys == null)
            {
                return;
            }

            var toDelete = command.Keys
                .Where(k => !string.IsNullOrEmpty(k))
                .Select(repository.GetByKey)
                .OfType<LocalizationResource>()
                .Where(r => !r.FromCode || command.IgnoreFromCode)
                .ToList();

            if (toDelete.Count == 0)
            {
                return;
            }

            repository.DeleteResources(toDelete);

            var cache = configurationContext.Value.CacheManager;
            foreach (var resource in toDelete)
            {
                cache.Remove(CacheKeyHelper.BuildKey(resource.ResourceKey));
            }
        }
    }

    /// <summary>
    /// Execute this command if you need to delete multiple resources at once. Resources synced from code
    /// are silently skipped unless <see cref="IgnoreFromCode"/> is set to <c>true</c>.
    /// </summary>
    /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
    public class Command : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        /// <param name="keys">Resource keys to delete.</param>
        public Command(IEnumerable<string> keys)
        {
            Keys = keys;
        }

        /// <summary>
        /// Resource keys to delete.
        /// </summary>
        public IEnumerable<string> Keys { get; }

        /// <summary>
        /// Set this flag to <c>true</c> to delete resources even when they are synced from code.
        /// </summary>
        public bool IgnoreFromCode { get; set; }
    }
}
