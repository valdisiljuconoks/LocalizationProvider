// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Commands;

/// <summary>
/// Updates the notes (comment) for an existing resource. Notes are per-resource (not per-language).
/// </summary>
public class UpdateResourceNotes
{
    /// <summary>
    /// Implementation for updating resource notes.
    /// </summary>
    public class Handler(IOptions<ConfigurationContext> configurationContext, IResourceRepository repository)
        : ICommandHandler<Command>
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument.
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(Command command)
        {
            ArgumentNullException.ThrowIfNull(command);

            var resource = repository.GetByKey(command.Key);
            if (resource == null)
            {
                return;
            }

            // intentionally not touching IsModified - editing a note must not freeze code-sync of translations
            resource.Notes = command.Notes;
            resource.ModificationDate = DateTime.UtcNow;

            repository.UpdateResource(resource);

            configurationContext.Value.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }

    /// <summary>
    /// Command definition for updating notes (comment) of an existing resource.
    /// </summary>
    /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
    public class Command : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="notes">The notes (comment) for the resource. Pass <c>null</c> to clear.</param>
        public Command(string key, string? notes)
        {
            Key = key;
            Notes = notes;
        }

        /// <summary>
        /// Gets the resource key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the notes (comment) for the resource.
        /// </summary>
        public string? Notes { get; }
    }
}
