// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Commands;

/// <summary>
/// Create or update translation for existing resource in given language.
/// </summary>
public class CreateOrUpdateTranslation
{
    /// <summary>
    /// Implementation for creating or updating existing translation
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
        public void Execute(Command command)
        {
            var resource = _repository.GetByKey(command.Key);
            var now = DateTime.UtcNow;

            if (resource == null)
            {
                return;
            }

            var translation = resource.Translations.FindByLanguage(command.Language);

            if (translation == null)
            {
                var newTranslation = new LocalizationResourceTranslation
                {
                    Value = command.Translation,
                    Language = command.Language.Name,
                    ResourceId = resource.Id,
                    ModificationDate = now
                };

                _repository.AddTranslation(resource, newTranslation);
            }
            else
            {
                translation.Value = command.Translation;
                translation.ModificationDate = now;
                _repository.UpdateTranslation(resource, translation);
            }

            resource.ModificationDate = now;
            resource.IsModified = true;

            _repository.UpdateResource(resource);

            _configurationContext.Value.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }

    /// <summary>
    /// Command definition for creating or updating translation for existing resource in given language.
    /// </summary>
    /// <seealso cref="DbLocalizationProvider.Abstractions.ICommand" />
    public class Command : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command" /> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="language">The language for the translation.</param>
        /// <param name="translation">The actual translation for given language.</param>
        public Command(string key, CultureInfo language, string translation)
        {
            Key = key;
            Language = language;
            Translation = translation;
        }

        /// <summary>
        /// Gets the resource key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the language.
        /// </summary>
        public CultureInfo Language { get; }

        /// <summary>
        /// Gets the translation for given <see cref="Language" />.
        /// </summary>
        public string Translation { get; }
    }
}
