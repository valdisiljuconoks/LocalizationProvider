// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands;

/// <summary>
/// This command is usually used when creating new resources either from AdminUI or during import process
/// </summary>
public class CreateNewResources
{
    /// <summary>
    /// Capture moment when somebody has created new resource in UI
    /// </summary>
    /// <param name="e">Arguments obviously to understand what has been created.</param>
    public delegate void EventHandler(EventArgs e);

    /// <summary>
    /// Implementation of the command to create new resources
    /// </summary>
    public class Handler : ICommandHandler<Command>
    {
        private readonly ConfigurationContext _configurationContext;
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        /// <param name="repository">Resource repository</param>
        public Handler(ConfigurationContext configurationContext, IResourceRepository repository)
        {
            _configurationContext = configurationContext;
            _repository = repository;
        }

        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        /// <exception cref="InvalidOperationException">Resource with key `{resource.ResourceKey}` already exists</exception>
        public void Execute(Command command)
        {
            if (command.LocalizationResources == null || !command.LocalizationResources.Any())
            {
                return;
            }

            foreach (var resource in command.LocalizationResources)
            {
                var existingResource = _repository.GetByKey(resource.ResourceKey);

                if (existingResource != null)
                {
                    throw new InvalidOperationException($"Resource with key `{resource.ResourceKey}` already exists");
                }

                resource.ModificationDate = DateTime.UtcNow;

                // if we are importing single translation and it's not invariant
                // set it also as invariant translation
                if (resource.Translations.Count == 1 && resource.Translations.InvariantTranslation() == null)
                {
                    var t = resource.Translations.First();
                    resource.Translations.Add(
                        new LocalizationResourceTranslation { Value = t.Value, Language = string.Empty });
                }

                _repository.InsertResource(resource);

                _configurationContext.BaseCacheManager.StoreKnownKey(resource.ResourceKey);
            }
        }
    }

    /// <summary>
    /// This command is usually used when creating new resources either from AdminUI or during import process, or somebody just figured out how to
    /// push new resources to db.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// Constructs new instance of command obviously.
        /// </summary>
        /// <param name="resources">List of resources to create</param>
        public Command(List<LocalizationResource> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            if (resources.Count == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(resources));
            }

            LocalizationResources = resources;
        }

        /// <summary>
        /// List of resources to create. Resource instance should be fully filled in order to just commit to underlying
        /// storage.
        /// </summary>
        public List<LocalizationResource> LocalizationResources { get; }
    }

    /// <summary>
    /// Arguments for the event handlers
    /// </summary>
    public class EventArgs : System.EventArgs
    {
        /// <summary>
        /// Creates new instance of argument class
        /// </summary>
        /// <param name="key">Resource key which has been created</param>
        public EventArgs(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Resource key which has been created
        /// </summary>
        public string Key { get; }
    }
}
